using System;

namespace ModelViewer.Contracts.Services;

public interface IPageService
{
    Type GetPageType(string key);
}
