using System;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using option;

namespace jonson.reflect {
    public static class Reflect {
        public static JSONType ToJSON<T>(T obj, bool ignoreNulls) {
            Option<JSONType> res = ObjToJSON(obj, ignoreNulls);
            if (res.IsNone()) {
                return JSONType.Make();
            }

            return res.Peel();
        }

        public static T FromJSON<T>(T obj, JSONType json) {
            return (T)JSONToObj(obj, json);
        }

        private static object JSONToObj(object obj, JSONType json) {
            if (json.Str.IsSome()) {
                obj = json.Str.Peel();
            } else if (json.Num.IsSome()) {
                string numStr = json.Num.Peel();
                NumberStyles style = NumberStyles.AllowLeadingSign;
                style |= NumberStyles.AllowDecimalPoint;
                style |= NumberStyles.AllowExponent;

                if (obj.GetType().IsEnum) {
                    obj = JSONToObj((int)obj, json);
                } else if (obj is byte) {
                    byte num;
                    if (byte.TryParse(numStr, style, CultureInfo.InvariantCulture, out num)) {
                        obj = num;
                    }
                } else if (obj is sbyte) {
                    sbyte num;
                    if (sbyte.TryParse(numStr, style, CultureInfo.InvariantCulture, out num)) {
                        obj = num;
                    }
                } else if (obj is decimal) {
                    decimal num;
                    if (decimal.TryParse(numStr, style, CultureInfo.InvariantCulture, out num)) {
                        obj = num;
                    }
                } else if (obj is double) {
                    double num;
                    if (double.TryParse(numStr, style, CultureInfo.InvariantCulture, out num)) {
                        obj = num;
                    }
                } else if (obj is float) {
                    float num;
                    if (float.TryParse(numStr, style, CultureInfo.InvariantCulture, out num)) {
                        obj = num;
                    }
                } else if (obj is int) {
                    int num;
                    if (int.TryParse(numStr, style, CultureInfo.InvariantCulture, out num)) {
                        obj = num;
                    }
                } else if (obj is uint) {
                    uint num;
                    if (uint.TryParse(numStr, style, CultureInfo.InvariantCulture, out num)) {
                        obj = num;
                    }
                } else if (obj is long) {
                    long num;
                    if (long.TryParse(numStr, style, CultureInfo.InvariantCulture, out num)) {
                        obj = num;
                    }
                } else if (obj is ulong) {
                    ulong num;
                    if (ulong.TryParse(numStr, style, CultureInfo.InvariantCulture, out num)) {
                        obj = num;
                    }
                } else if (obj is short) {
                    short num;
                    if (short.TryParse(numStr, style, CultureInfo.InvariantCulture, out num)) {
                        obj = num;
                    }
                } else if (obj is ushort) {
                    ushort num;
                    if (ushort.TryParse(numStr, style, CultureInfo.InvariantCulture, out num)) {
                        obj = num;
                    }
                }
            } else if (json.Bool.IsSome()) {
                if (obj is bool) {
                    obj = json.Bool.Peel();
                }
            } else if (json.Obj.IsSome()) {
                if (obj == null) {
                    return null;
                }
                Type objType = obj.GetType();
                Dictionary<string, JSONType> jsonDict = json.Obj.Peel();
                if (obj is IDictionary && objType.IsGenericType) {
                    Type[] args = objType.GetGenericArguments();
                    if (args[0] == typeof(string)) {
                        object value;
                        if (args[1] == typeof(string)) {
                            value = "";
                        } else {
                            value = Activator.CreateInstance(args[1]);
                        }
                        IDictionary dict = (IDictionary)obj;
                        foreach (string key in jsonDict.Keys) {
                            dict[key] = JSONToObj(value, jsonDict[key]);
                        }
                    }
                } else {
                    BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
                    FieldInfo[] fields = obj.GetType().GetFields(flags);

                    for (int i = 0; i < fields.Length; i++) {
                        FieldInfo field = fields[i];
                        if (jsonDict.ContainsKey(field.Name)) {
                            object fieldValue = field.GetValue(obj);
                            fieldValue = JSONToObj(fieldValue, jsonDict[field.Name]);
                            field.SetValue(obj, fieldValue);
                        }
                    }
                }
            } else if (json.Arr.IsSome()) {
                if (obj == null) {
                    return null;
                }
                Type objType = obj.GetType();
                List<JSONType> jsonArr = json.Arr.Peel();
                if (obj is IList) {
                    if (objType.IsArray || objType.IsGenericType) {
                        Type elementType;
                        bool isArray = objType.IsArray;
                        if (isArray) {
                            elementType = objType.GetElementType();
                            obj = Array.CreateInstance(elementType, jsonArr.Count);
                        } else {
                            Type[] genericArgs = objType.GenericTypeArguments;
                            elementType = genericArgs[0];
                            Type listType = typeof(List<>);
                            Type concreteType = listType.MakeGenericType(genericArgs);
                            obj = Activator.CreateInstance(concreteType);
                        }

                        IList list = (IList)obj;
                        for (int i = 0; i < jsonArr.Count; i++) {
                            object value;
                            if (elementType == typeof(string)) {
                                value = "";
                            } else {
                                value = Activator.CreateInstance(elementType);
                            }
                            value = JSONToObj(value, jsonArr[i]);
                            if (isArray) {
                                list[i] = value;
                            } else {
                                list.Add(value);
                            }
                        }
                    }
                }
            } else if (json.Null.IsSome()) {
                obj = null;
            }

            return obj;
        }

        private static Option<JSONType> ObjToJSON(object obj, bool ignoreNulls) {
            if (obj == null) {
                if (ignoreNulls) {
                    return Option<JSONType>.None();
                }

                return Option<JSONType>.Some(JSONType.Make());
            }

            Type type = obj.GetType();

            if (type == typeof(string)) {
                return Option<JSONType>.Some(JSONType.Make(obj as string));
            } else if (type == typeof(char)) {
                return Option<JSONType>.Some(JSONType.Make(obj.ToString()));
            } else if (type == typeof(bool)) {
                return Option<JSONType>.Some(JSONType.Make((bool)(object)obj));
            } else if (type.IsEnum) {
                object enumVal = System.Convert.ChangeType(obj, Enum.GetUnderlyingType(type));
                return ObjToJSON(enumVal, ignoreNulls);
            } else {
                JSONType numType = new JSONType();
                if (obj is byte) {
                    byte num = (byte)obj;
                    numType.Num = Option<string>.Some(num.ToString(CultureInfo.InvariantCulture));
                    return Option<JSONType>.Some(numType);
                } else if (obj is sbyte) {
                    sbyte num = (sbyte)obj;
                    numType.Num = Option<string>.Some(num.ToString(CultureInfo.InvariantCulture));
                    return Option<JSONType>.Some(numType);
                } else if (obj is decimal) {
                    decimal num = (decimal)obj;
                    numType.Num = Option<string>.Some(num.ToString(CultureInfo.InvariantCulture));
                    return Option<JSONType>.Some(numType);
                } else if (obj is double) {
                    double num = (double)obj;
                    numType.Num = Option<string>.Some(num.ToString(CultureInfo.InvariantCulture));
                    return Option<JSONType>.Some(numType);
                } else if (obj is float) {
                    float num = (float)obj;
                    numType.Num = Option<string>.Some(num.ToString(CultureInfo.InvariantCulture));
                    return Option<JSONType>.Some(numType);
                } else if (obj is int) {
                    int num = (int)obj;
                    numType.Num = Option<string>.Some(num.ToString(CultureInfo.InvariantCulture));
                    return Option<JSONType>.Some(numType);
                } else if (obj is uint) {
                    uint num = (uint)obj;
                    numType.Num = Option<string>.Some(num.ToString(CultureInfo.InvariantCulture));
                    return Option<JSONType>.Some(numType);
                } else if (obj is long) {
                    long num = (long)obj;
                    numType.Num = Option<string>.Some(num.ToString(CultureInfo.InvariantCulture));
                    return Option<JSONType>.Some(numType);
                } else if (obj is ulong) {
                    ulong num = (ulong)obj;
                    numType.Num = Option<string>.Some(num.ToString(CultureInfo.InvariantCulture));
                    return Option<JSONType>.Some(numType);
                } else if (obj is short) {
                    short num = (short)obj;
                    numType.Num = Option<string>.Some(num.ToString(CultureInfo.InvariantCulture));
                    return Option<JSONType>.Some(numType);
                } else if (obj is ushort) {
                    ushort num = (ushort)obj;
                    numType.Num = Option<string>.Some(num.ToString(CultureInfo.InvariantCulture));
                    return Option<JSONType>.Some(numType);
                } else if (obj is IDictionary) {
                    IDictionary objDict = (IDictionary)obj;
                    Dictionary<string, JSONType> dict = new Dictionary<string, JSONType>();
                    foreach (object key in objDict.Keys) {
                        Option<JSONType> val = ObjToJSON(objDict[key], ignoreNulls);
                        if (val.IsSome() || !ignoreNulls) {
                            dict[key.ToString()] = val.Peel();
                        }
                    }
                    return Option<JSONType>.Some(JSONType.Make(dict));
                } else if (obj is IList) {
                    List<JSONType> list = new List<JSONType>();
                    IEnumerable arr = (IEnumerable)(object)obj;
                    foreach (object element in arr) {
                        Option<JSONType> val = ObjToJSON(element, ignoreNulls);
                        if (val.IsSome() || !ignoreNulls) {
                            list.Add(val.Peel());
                        }
                    }
                    return Option<JSONType>.Some(JSONType.Make(list));
                } else {
                    BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
                    FieldInfo[] fields = type.GetFields(flags);
                    Dictionary<string, JSONType> dict = new Dictionary<string, JSONType>();
                    for (int i = 0; i < fields.Length; i++) {
                        FieldInfo field = fields[i];
                        Option<JSONType> val = ObjToJSON(field.GetValue(obj), ignoreNulls);
                        if (val.IsSome() || !ignoreNulls) {
                            dict[field.Name] = val.Peel();
                        }
                    }

                    return Option<JSONType>.Some(JSONType.Make(dict));
                }
            }
        }
    }
}
