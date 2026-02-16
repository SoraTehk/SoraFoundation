using System.Runtime.CompilerServices;

#if USE_ZSTRING
using Cysharp.Text;
#endif

namespace SoraTehk.Extensions {
    public static partial class StringX {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RTColorByHash(this string str) {
            if (string.IsNullOrEmpty(str)) return str;

            int hash = str.GetHashCode();

            byte r = (byte)((hash >> 16) & 0xFF);
            byte g = (byte)((hash >> 8) & 0xFF);
            byte b = (byte)(hash & 0xFF);

#if USE_ZSTRING
            return ZString.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", r, g, b, str);
#else
            return $"<color=#{r:X2}{g:X2}{b:X2}>{str}</color>";
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RTColorByHash(this string str, string color) {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(color)) return str;

            int hash = color.GetHashCode();

            byte r = (byte)((hash >> 16) & 0xFF);
            byte g = (byte)((hash >> 8) & 0xFF);
            byte b = (byte)(hash & 0xFF);

#if USE_ZSTRING
            return ZString.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", r, g, b, str);
#else
            return $"<color=#{r:X2}{g:X2}{b:X2}>{str}</color>";
#endif
        }
    }
}