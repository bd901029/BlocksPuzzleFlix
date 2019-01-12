#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("5tyc8iD11TQad/zN7mnvbQZaBAxpiUnfGXXYSfyG0pfm+VU/tPtA3fFyfHND8XJ5cfFycnPKvyCCyLKhn5kxgwUisnXSOs4Xj7035wVaIGlD8XJRQ351eln1O/WEfnJycnZzcJeHk7Jef8NIZcj6gkj6sQE9qz6S2qFfjQozduGl4lnXZgpZYY3FC3tVuzs7c6modCxPzUIBhodd4u/9IgFyHqYX2SY7hBjdmNeE6DKXHH9R2eUVtdiAN3RGAAQmRYl2rB8/c1uuyZB/26FdpKhfhOASt/Pnz7yrTvP0wvHn5W4Ro+i8TpajdZu9PJJAnh5uICUXT/c+hOcfj3bFHt8lTgo1FbfODqQ4N2YdkobNJTq3y1QYFIwajL9N1ycC6nFwcnNy");
        private static int[] order = new int[] { 7,12,12,11,7,8,10,10,10,12,12,13,12,13,14 };
        private static int key = 115;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
