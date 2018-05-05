//--------------------------------------------------------------------------------------
// Copyright 2014 Intel Corporation
// All Rights Reserved
//
// Permission is granted to use, copy, distribute and prepare derivative works of this
// software for any purpose and without fee, provided, that the above copyright notice
// and this statement appear in all copies.  Intel makes no representations about the
// suitability of this software for any purpose.  THIS SOFTWARE IS PROVIDED "AS IS."
// INTEL SPECIFICALLY DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, AND ALL LIABILITY,
// INCLUDING CONSEQUENTIAL AND OTHER INDIRECT DAMAGES, FOR THE USE OF THIS SOFTWARE,
// INCLUDING LIABILITY FOR INFRINGEMENT OF ANY PROPRIETARY RIGHTS, AND INCLUDING THE
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.  Intel does not
// assume any responsibility for any errors which may appear in this software nor any
// responsibility to update it.
//
// Created by: filip.strugar@intel.com
//--------------------------------------------------------------------------------------
//
// - function "GenerateGaussShaderKernelWeightsAndOffsets" will generate shader 
//      constants for high performance Gaussian blur filter implementation (separable, 
//      using hardware linear filter when sampling to get two samples at a time)
//
// - function "GenerateGaussFunctionCode" will generate GLSL code using the above
//      constants; for HLSL replace vec2/vec3 with float2/float3, etc, "should work"
//
// - Acceptable kernel sizes are of 4*n-1 (3, 7, 11, 15, ...)
//
// - Gauss distribution Sigma value is generated "ad hoc": feel free to modify the code
//      for your purpose. Performance of the algorithm is only dependent on kernel size.
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.SharpDX.ShaderBuilder
{
    public static class GaussianBlurKernelValueGenerator
    {
        private static IList<double> GenerateSeparableGaussKernel(double sigma, int kernelSize)
        {
            if ((kernelSize % 2) != 1)
            {// kernel size must be odd number
                throw new ArgumentException("kernel size must be odd number");
            }

            int halfKernelSize = kernelSize / 2;

            var kernel = new double[kernelSize];

            const double cPI = 3.14159265358979323846;
            double mean = halfKernelSize;
            double sum = 0.0;
            for (int x = 0; x < kernelSize; ++x)
            {
                kernel[x] = (float)Math.Sqrt(Math.Exp(-0.5 * (Math.Pow((x - mean) / sigma, 2.0) + Math.Pow((mean) / sigma, 2.0)))
                    / (2 * cPI * sigma * sigma));
                sum += kernel[x];
            }
            for (int x = 0; x < kernelSize; ++x)
                kernel[x] /= (float)sum;

            return kernel;
        }

        private static IList<float> GetAppropriateSeparableGauss(int kernelSize, double sigma = 1.0)
        {
            if ((kernelSize % 2) != 1)
            {
                throw new ArgumentException("kernel size must be odd number");
            }

            // Search for sigma to cover the whole kernel size with sensible values (might not be ideal for all cases quality-wise but is good enough for performance testing)
            double epsilon = 2e-2f / kernelSize;
            double searchStep = 1.0;
            while (true)
            {

                var kernelAttempt = GenerateSeparableGaussKernel(sigma, kernelSize);
                if (kernelAttempt[0] > epsilon)
                {
                    if (searchStep > 0.02)
                    {
                        sigma -= searchStep;
                        searchStep *= 0.1;
                        sigma += searchStep;
                        continue;
                    }
                    var retVal = new List<float>();
                    for (int i = 0; i < kernelSize; i++)
                        retVal.Add((float)kernelAttempt[i]);
                    return retVal;
                }

                sigma += searchStep;

                if (sigma > 1000.0)
                {
                    return new List<float>();
                }
            }
        }

        public static void GetKernel(int kernelSize, double sigma, out IList<float> offsets, out IList<float> weights)
        {
            // Gauss filter kernel & offset creation
            var inputKernel = GetAppropriateSeparableGauss(kernelSize, sigma);

            var oneSideInputs = new List<float>();
            for (int i = (kernelSize / 2); i >= 0; i--)
            {
                if (i == (kernelSize / 2))
                    oneSideInputs.Add((float)inputKernel[i] * 0.5f);
                else
                    oneSideInputs.Add((float)inputKernel[i]);
            }

            int numSamples = oneSideInputs.Count / 2;

            weights = new List<float>();

            for (int i = 0; i < numSamples; i++)
            {
                float sum = oneSideInputs[i * 2 + 0] + oneSideInputs[i * 2 + 1];
                weights.Add(sum);
            }

            offsets = new List<float>();

            for (int i = 0; i < numSamples; i++)
            {
                offsets.Add(i * 2.0f + oneSideInputs[i * 2 + 1] / weights[i]);
            }
        }
    }
}
