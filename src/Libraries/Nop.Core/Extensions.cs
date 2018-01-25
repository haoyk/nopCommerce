﻿using System;
﻿using System.Xml;

namespace Nop.Core
{
    public static class Extensions
    {
        public static bool IsNullOrDefault<T>(this T? value) where T : struct
        {
            return default(T).Equals(value.GetValueOrDefault());
        }  

        public static string ElText(this XmlNode node, string elName)
        {
            return node.SelectSingleNode(elName).InnerText;
        }

        public static TResult Return<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator, TResult failureValue)
            where TInput : class
        {
            return o == null ? failureValue : evaluator(o);
        }

        #region MyRegion

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static void TrueThrow(this bool value, string msg)
        {
            if (value == true)
            {
                throw new Exception(msg);
            }
        }

        public static void FalseThrow(this bool value, string msg)
        {
            if (value == false)
            {
                throw new Exception(msg);
            }
        }

        public static void NullOrEmptyCheck(this string value, string fieldName)
        {
            value.IsNullOrEmpty().TrueThrow(string.Format("字段{0}不能为空", fieldName));
        }

        public static void NullCheck(this object value, string fieldName)
        {
            (value == null).TrueThrow(string.Format("字段{0}不能为Null", fieldName));
        }

        public static bool IsMinValue(this DateTime value)
        {
            return DateTime.MinValue == value;
        }

        #endregion
    }
}
