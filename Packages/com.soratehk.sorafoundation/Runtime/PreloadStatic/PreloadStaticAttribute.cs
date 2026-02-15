using System;

namespace SoraTehk.Attributes {
    [Flags]
    public enum PreloadStaticModes {
        NONE = 0,
        EDIT_MODE = 1 << 0,
        PLAY_MODE = 1 << 1,
        BOTH = EDIT_MODE | PLAY_MODE,
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public class PreloadStaticAttribute : Attribute {
        public Type[]? AfterTypes { get; set; } = null;
        public Type[]? BeforeTypes { get; set; } = null;
        public PreloadStaticModes Modes { get; set; } = PreloadStaticModes.BOTH;
    }
}