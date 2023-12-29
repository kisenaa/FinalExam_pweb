﻿using System.Collections.Specialized;
using System.Web;

namespace Client.Utils;

public static class UriExtensions
{
    public static Uri AddParameter(this Uri url, params (string Name, string Value)[] @params)
    {
        if (!@params.Any())
        {
            return url;
        }

        UriBuilder uriBuilder = new(url);

        NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

        foreach (var param in @params)
        {
            query[param.Name] = param.Value.Trim();
        }

        uriBuilder.Query = query.ToString();

        return uriBuilder.Uri;
    }
}