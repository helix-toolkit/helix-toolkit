/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Diagnostics;
using System.Globalization;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Utilities
    {
        public class TokenizerHelper
        {
            private char quoteChar;
            private char argSeparator;
            private string str;
            private int strLen;
            private int charIndex;
            private int currentTokenIndex;
            private int currentTokenLength;
            private bool foundSeparator;

            /// <summary> 
            /// Constructor for TokenizerHelper which accepts an IFormatProvider.
            /// If the IFormatProvider is null, we use the thread's IFormatProvider info. 
            /// We will use ',' as the list separator, unless it's the same as the
            /// decimal separator.  If it *is*, then we can't determine if, say, "23,5" is one
            /// number or two.  In this case, we will use ";" as the separator.
            /// </summary> 
            /// <param name="str"> The string which will be tokenized. </param>
            /// <param name="formatProvider"> The IFormatProvider which controls this tokenization. </param> 
            public TokenizerHelper(string str, IFormatProvider formatProvider)
            {
                char numberSeparator = GetNumericListSeparator(formatProvider);
                this.Initialize(str, '\'', numberSeparator);
            }

            /// <summary>
            /// Initialize the TokenizerHelper with the string to tokenize,
            /// the char which represents quotes and the list separator.
            /// </summary> 
            /// <param name="str"> The string to tokenize. </param>
            /// <param name="quoteChar"> The quote char. </param> 
            /// <param name="separator"> The list separator. </param> 
            public TokenizerHelper(string str, char quoteChar, char separator)
            {
                this.Initialize(str, quoteChar, separator);
            }

            /// <summary>
            /// Initialize the TokenizerHelper with the string to tokenize,
            /// the char which represents quotes and the list separator.
            /// </summary> 
            /// <param name="str"> The string to tokenize. </param>
            /// <param name="quoteChar"> The quote char. </param> 
            /// <param name="separator"> The list separator. </param> 
            private void Initialize(string str, char quoteChar, char separator)
            {
                this.str = str;
                this.strLen = str == null ? 0 : str.Length;
                this.currentTokenIndex = -1;
                this.quoteChar = quoteChar;
                this.argSeparator = separator;

                // immediately forward past any whitespace so 
                // NextToken() logic always starts on the first
                // character of the next token.
                while (this.charIndex < this.strLen)
                {
                    if (!Char.IsWhiteSpace(this.str, this.charIndex))
                    {
                        break;
                    }

                    ++this.charIndex;
                }
            }

            public string GetCurrentToken()
            {
                // if no current token, return null 
                if (this.currentTokenIndex < 0)
                {
                    return null;
                }

                return this.str.Substring(this.currentTokenIndex, this.currentTokenLength);
            }

            /// <summary> 
            /// Throws an exception if there is any non-whitespace left un-parsed.
            /// </summary> 
            public void LastTokenRequired()
            {
                if (this.charIndex != this.strLen)
                {
                    throw new InvalidOperationException("TokenizerHelperExtraDataEncountered");
                }
            }

            /// <summary> 
            /// Advances to the NextToken
            /// </summary>
            /// <returns>true if next token was found, false if at end of string</returns>
            public bool NextToken()
            {
                return NextToken(false);
            }

            /// <summary> 
            /// Advances to the NextToken, throwing an exception if not present
            /// </summary>
            /// <returns>The next token found</returns>
            public string NextTokenRequired()
            {
                if (!NextToken(false))
                {
                    throw new InvalidOperationException("TokenizerHelperPrematureStringTermination");
                }

                return GetCurrentToken();
            }

            /// <summary>
            /// Advances to the NextToken, throwing an exception if not present 
            /// </summary> 
            /// <returns>The next token found</returns>
            public string NextTokenRequired(bool allowQuotedToken)
            {
                if (!NextToken(allowQuotedToken))
                {
                    throw new InvalidOperationException("TokenizerHelperPrematureStringTermination");
                }

                return GetCurrentToken();
            }

            /// <summary>
            /// Advances to the NextToken
            /// </summary>
            /// <returns>true if next token was found, false if at end of string</returns> 
            public bool NextToken(bool allowQuotedToken)
            {
                // use the currently-set separator character. 
                return NextToken(allowQuotedToken, this.argSeparator);
            }

            /// <summary>
            /// Advances to the NextToken.  A separator character can be specified
            /// which overrides the one previously set. 
            /// </summary>
            /// <returns>true if next token was found, false if at end of string</returns> 
            public bool NextToken(bool allowQuotedToken, char separator)
            {
                this.currentTokenIndex = -1; // reset the currentTokenIndex 
                this.foundSeparator = false; // reset

                // If we're at end of the string, just return false.
                if (this.charIndex >= this.strLen)
                {
                    return false;
                }

                char currentChar = this.str[this.charIndex];

                Debug.Assert(!Char.IsWhiteSpace(currentChar), "Token started on Whitespace");

                // setup the quoteCount 
                int quoteCount = 0;

                // If we are allowing a quoted token and this token begins with a quote, 
                // set up the quote count and skip the initial quote
                if (allowQuotedToken &&
                    currentChar == this.quoteChar)
                {
                    quoteCount++; // increment quote count
                    ++this.charIndex; // move to next character 
                }

                int newTokenIndex = this.charIndex;
                int newTokenLength = 0;

                // loop until hit end of string or hit a , or whitespace
                // if at end of string ust return false.
                while (this.charIndex < this.strLen)
                {
                    currentChar = this.str[this.charIndex];

                    // if have a QuoteCount and this is a quote 
                    // decrement the quoteCount
                    if (quoteCount > 0)
                    {
                        // if anything but a quoteChar we move on
                        if (currentChar == this.quoteChar)
                        {
                            --quoteCount;

                            // if at zero which it always should for now 
                            // break out of the loop
                            if (0 == quoteCount)
                            {
                                ++this.charIndex; // move past the quote
                                break;
                            }
                        }
                    }
                    else if ((Char.IsWhiteSpace(currentChar)) || (currentChar == separator))
                    {
                        if (currentChar == separator)
                        {
                            this.foundSeparator = true;
                        }
                        break;
                    }

                    ++this.charIndex;
                    ++newTokenLength;
                }

                // if quoteCount isn't zero we hit the end of the string
                // before the ending quote
                if (quoteCount > 0)
                {
                    throw new InvalidOperationException("TokenizerHelperMissingEndQuote");
                }

                ScanToNextToken(separator); // move so at the start of the nextToken for next call 

                // finally made it, update the _currentToken values
                this.currentTokenIndex = newTokenIndex;
                this.currentTokenLength = newTokenLength;

                if (this.currentTokenLength < 1)
                {
                    throw new InvalidOperationException("TokenizerHelperEmptyToken");
                }

                return true;
            }

            // helper to move the _charIndex to the next token or to the end of the string
            private void ScanToNextToken(char separator)
            {
                // if already at end of the string don't bother
                if (this.charIndex < this.strLen)
                {
                    char currentChar = this.str[this.charIndex];

                    // check that the currentChar is a space or the separator.  If not 
                    // we have an error. this can happen in the quote case
                    // that the char after the quotes string isn't a char. 
                    if (!(currentChar == separator) &&
                        !Char.IsWhiteSpace(currentChar))
                    {
                        throw new InvalidOperationException("TokenizerHelperExtraDataEncountered");
                    }

                    // loop until hit a character that isn't 
                    // an argument separator or whitespace.
                    // !!!Todo: if more than one argSet throw an exception 
                    int argSepCount = 0;
                    while (this.charIndex < this.strLen)
                    {
                        currentChar = this.str[this.charIndex];

                        if (currentChar == separator)
                        {
                            this.foundSeparator = true;
                            ++argSepCount;
                            this.charIndex++;

                            if (argSepCount > 1)
                            {
                                throw new InvalidOperationException("TokenizerHelperEmptyToken");
                            }
                        }
                        else if (Char.IsWhiteSpace(currentChar))
                        {
                            ++this.charIndex;
                        }
                        else
                        {
                            break;
                        }
                    }

                    // if there was a separatorChar then we shouldn't be 
                    // at the end of string or means there was a separator 
                    // but there isn't an arg

                    if (argSepCount > 0 && this.charIndex >= this.strLen)
                    {
                        throw new InvalidOperationException("TokenizerHelperEmptyToken");
                    }
                }
            }

            // Helper to get the numeric list separator for a given IFormatProvider.
            // Separator is a comma [,] if the decimal separator is not a comma, or a semicolon [;] otherwise. 
            public static char GetNumericListSeparator(IFormatProvider provider)
            {
                char numericSeparator = ',';

                // Get the NumberFormatInfo out of the provider, if possible
                // If the IFormatProvider doesn't not contain a NumberFormatInfo, then 
                // this method returns the current culture's NumberFormatInfo. 
                NumberFormatInfo numberFormat = NumberFormatInfo.GetInstance(provider);

                Debug.Assert(null != numberFormat);

                // Is the decimal separator is the same as the list separator?
                // If so, we use the ";". 
                if ((numberFormat.NumberDecimalSeparator.Length > 0) && (numericSeparator == numberFormat.NumberDecimalSeparator[0]))
                {
                    numericSeparator = ';';
                }

                return numericSeparator;
            }

            public bool FoundSeparator
            {
                get
                {
                    return this.foundSeparator;
                }
            }
        }
    }

}
