﻿using System;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.SkiaSharp
{
	public class Platform : BindableObject, IPlatform, IDisposable
	{
		bool _disposed;
		readonly PlatformRenderer _renderer;

		public Platform()
		{
			_renderer = new PlatformRenderer(this);
		}

		internal static readonly BindableProperty RendererProperty =
		BindableProperty.CreateAttached("Renderer", typeof(IVisualElementRenderer),
			typeof(Platform), default(IVisualElementRenderer),
		propertyChanged: (bindable, oldvalue, newvalue) =>
		{
			if (bindable is VisualElement view)
				view.IsPlatformEnabled = newvalue != null;
		});

		Page Page { get; set; }

		public PlatformRenderer PlatformRenderer => _renderer;

		public void Dispose()
		{
			if (_disposed)
				return;

			_disposed = true;
		}

		SizeRequest IPlatform.GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
		{
			var renderView = GetRenderer(view);

			if (renderView == null || renderView.Control == null)
				return new SizeRequest(Size.Zero);

			return renderView.GetDesiredSize(widthConstraint, heightConstraint);
		}

		public static IVisualElementRenderer GetRenderer(VisualElement element)
		{
			return (IVisualElementRenderer)element.GetValue(RendererProperty);
		}

		public static IVisualElementRenderer CreateRenderer(VisualElement element)
		{
			var elementType = element.GetType();

			var renderer =
				Registrar.Registered.GetHandler<IVisualElementRenderer>(elementType) ??
				new DefaultRenderer();

			renderer.SetElement(element);

			return renderer;
		}

		public static void SetRenderer(VisualElement element, IVisualElementRenderer value)
		{
			if (element != null)
			{
				element.SetValue(RendererProperty, value);
				element.IsPlatformEnabled = value != null;
			}
		}

		public void SetPage(Page newRoot)
		{
			if (newRoot == null)
				return;

			if (Page != null)
				throw new NotImplementedException();

			Page = newRoot;
			Page.Platform = this;

			AddChild(Page);
		}

		void AddChild(Page mainPage)
		{
			var viewRenderer = GetRenderer(mainPage);

			if (viewRenderer == null)
			{
				viewRenderer = CreateRenderer(mainPage);
				SetRenderer(mainPage, viewRenderer);

				PlatformRenderer.AddView(viewRenderer.Control);

				// TODO: This is more of a hacky workaround than what I should be doing.
				_renderer.Invalidated += (s,e) =>
				{
					viewRenderer.SetElementSize(new Size(_renderer.Frame.Width, _renderer.Frame.Height));
				};
				
				viewRenderer.SetElementSize(new Size(640, 480));
			}
		}
	}

	internal class DefaultRenderer : VisualElementRenderer<VisualElement, Native.SKView>
	{
		internal DefaultRenderer() => SetNativeControl(new Native.SKView());
	}
}
