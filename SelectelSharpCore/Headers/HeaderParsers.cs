using SelectelSharpCore.Headers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;

namespace SelectelSharpCore.Headers {
    internal static class HeaderParsers {
        internal static int ToInt(string value) {
            int result;
            return int.TryParse(value, out result) ? result : 0;
        }

        internal static long ToLong(string value) {
            long result;
            return long.TryParse(value, out result) ? result : 0;
        }

        internal static T ParseHeaders<T>(HttpResponseHeaders headers)
            where T : new() {

            var obj = new T();
            ParseHeaders(obj, headers);
            return obj;
        }

        internal static void ParseHeaders(object obj, HttpResponseHeaders headers) {
            var props = obj
                .GetType()
                .GetRuntimeProperties();

            foreach (var prop in props) {
                var headerAttr = GetAttribute<HeaderAttribute>(prop);
                if (headerAttr != null) {
                    if (headerAttr.CustomHeaders) {
                        var customHeadersKeys = headers

                            .Where(x => x.Key.ToLower().StartsWith(HeaderKeys.XContainerMetaPrefix.ToLower()))
                            .Where(x => x.Key.ToLower() != HeaderKeys.XContainerMetaType.ToLower())
                            .Where(x => x.Key.ToLower() != HeaderKeys.XContainerMetaGallerySecret.ToLower())
                            .Select(_x => _x.Key);

                        if (customHeadersKeys.Any()) {
                            var customHeaders = new Dictionary<string, string>();
                            foreach (var key in customHeadersKeys) {
                                customHeaders.Add(key, headers.GetValues(key).FirstOrDefault());
                            }
                            prop.SetValue(obj, customHeaders);
                        }
                    } else if (headerAttr.CORSHeaders) {
                        var cors = new CORSHeaders(headers);
                        prop.SetValue(obj, cors);
                    } else {
                        if (!headers.TryGetValues(headerAttr.HeaderKey, out IEnumerable<string> values))
                            continue;
                        var value = values.FirstOrDefault();
                        if (value == null)
                            continue;

                        if (value.GetType().Equals(prop.PropertyType)) {
                            prop.SetValue(obj, value);
                        } else {
                            try {
                                var convertedValue = Convert.ChangeType(value, prop.PropertyType);
                                prop.SetValue(obj, convertedValue);
                            } catch { }
                        }
                    }
                }
            }
        }

        private static T GetAttribute<T>(PropertyInfo pi)
            where T : Attribute {
            var attr = pi.GetCustomAttributes<T>();
            if (attr.Any()) {
                return attr.First();
            } else {
                return null;
            }
        }
    }
}
