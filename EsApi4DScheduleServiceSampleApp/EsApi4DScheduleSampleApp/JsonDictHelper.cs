/*---------------------------------------------------------------------------------------------
* Copyright (c) Bentley Systems, Incorporated. All rights reserved.
* See LICENSE.md in the project root for license terms and full copyright notice.
*--------------------------------------------------------------------------------------------*/
using Newtonsoft.Json.Linq;

namespace EsApi4DScheduleSampleApp
{
    public class JsonDictHelper
    {
        public static Dictionary<string, object> DeserialiseAndFlatten(string json)
        {
            Dictionary<string, object> jsonAsDictionary = new Dictionary<string, object>();
            JToken token = JToken.Parse(json);
            FillDictionary(jsonAsDictionary, token, "");
            return jsonAsDictionary;
        }

        private static void FillDictionary(Dictionary<string, object> dictionary, JToken token, string prefix)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    foreach (JProperty prop in token.Children<JProperty>())
                    {
                        FillDictionary(dictionary, prop.Value, Join(prefix, prop.Name));
                    }
                    break;

                case JTokenType.Array:
                    int index = 0;
                    foreach (JToken value in token.Children())
                    {
                        FillDictionary(dictionary, value, Join(prefix, index.ToString()));
                        index++;
                    }
                    break;

                default:
                    dictionary.Add(prefix, ((JValue)token).Value!);
                    break;
            }
        }

        private static string Join(string prefix, string name)
        {
            return string.IsNullOrEmpty(prefix) ? name : prefix + "." + name;
        }
    }
}
