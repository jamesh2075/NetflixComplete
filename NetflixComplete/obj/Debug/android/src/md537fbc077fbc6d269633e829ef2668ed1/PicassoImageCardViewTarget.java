package md537fbc077fbc6d269633e829ef2668ed1;


public class PicassoImageCardViewTarget
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.squareup.picasso.Target
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onBitmapFailed:(Landroid/graphics/drawable/Drawable;)V:GetOnBitmapFailed_Landroid_graphics_drawable_Drawable_Handler:Squareup.Picasso.ITargetInvoker, Picasso\n" +
			"n_onBitmapLoaded:(Landroid/graphics/Bitmap;Lcom/squareup/picasso/Picasso$LoadedFrom;)V:GetOnBitmapLoaded_Landroid_graphics_Bitmap_Lcom_squareup_picasso_Picasso_LoadedFrom_Handler:Squareup.Picasso.ITargetInvoker, Picasso\n" +
			"n_onPrepareLoad:(Landroid/graphics/drawable/Drawable;)V:GetOnPrepareLoad_Landroid_graphics_drawable_Drawable_Handler:Squareup.Picasso.ITargetInvoker, Picasso\n" +
			"";
		mono.android.Runtime.register ("NetflixComplete.DroidTV.PicassoImageCardViewTarget, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", PicassoImageCardViewTarget.class, __md_methods);
	}


	public PicassoImageCardViewTarget () throws java.lang.Throwable
	{
		super ();
		if (getClass () == PicassoImageCardViewTarget.class)
			mono.android.TypeManager.Activate ("NetflixComplete.DroidTV.PicassoImageCardViewTarget, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public PicassoImageCardViewTarget (android.support.v17.leanback.widget.ImageCardView p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == PicassoImageCardViewTarget.class)
			mono.android.TypeManager.Activate ("NetflixComplete.DroidTV.PicassoImageCardViewTarget, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Support.V17.Leanback.Widget.ImageCardView, Xamarin.Android.Support.v17.Leanback, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}


	public void onBitmapFailed (android.graphics.drawable.Drawable p0)
	{
		n_onBitmapFailed (p0);
	}

	private native void n_onBitmapFailed (android.graphics.drawable.Drawable p0);


	public void onBitmapLoaded (android.graphics.Bitmap p0, com.squareup.picasso.Picasso.LoadedFrom p1)
	{
		n_onBitmapLoaded (p0, p1);
	}

	private native void n_onBitmapLoaded (android.graphics.Bitmap p0, com.squareup.picasso.Picasso.LoadedFrom p1);


	public void onPrepareLoad (android.graphics.drawable.Drawable p0)
	{
		n_onPrepareLoad (p0);
	}

	private native void n_onPrepareLoad (android.graphics.drawable.Drawable p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
