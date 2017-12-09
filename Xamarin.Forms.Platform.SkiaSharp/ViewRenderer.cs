﻿using NativeView = Xamarin.Forms.Platform.SkiaSharp.Controls.Control;

namespace Xamarin.Forms.Platform.SkiaSharp
{
    public abstract class ViewRenderer : ViewRenderer<View, NativeView>
    {

    }

    public abstract class ViewRenderer<TView, TNativeView> : VisualElementRenderer<TView, TNativeView>
      where TView : View where TNativeView : NativeView
    {
        protected override void SetNativeControl(TNativeView view)
        {
            base.SetNativeControl(view);

            Controls.Add(view);
        }
    }
}
