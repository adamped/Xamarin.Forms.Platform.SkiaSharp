﻿namespace Xamarin.Forms.Platform.SkiaSharp
{
    public class PlatformRenderer : Native.SKView
    {
        Platform _platform;

        public PlatformRenderer(Platform platform)
        {
            SetPlatform(platform);
        }

        public void SetPlatform(Platform value)
        {
            _platform = value;
        }
    }
}