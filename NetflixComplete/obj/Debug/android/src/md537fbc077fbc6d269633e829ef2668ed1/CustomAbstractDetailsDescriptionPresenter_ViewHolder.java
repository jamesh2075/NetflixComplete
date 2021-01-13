package md537fbc077fbc6d269633e829ef2668ed1;


public class CustomAbstractDetailsDescriptionPresenter_ViewHolder
	extends android.support.v17.leanback.widget.Presenter.ViewHolder
	implements
		mono.android.IGCUserPeer,
		android.view.View.OnLayoutChangeListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onLayoutChange:(Landroid/view/View;IIIIIIII)V:GetOnLayoutChange_Landroid_view_View_IIIIIIIIHandler:Android.Views.View/IOnLayoutChangeListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("NetflixComplete.DroidTV.CustomAbstractDetailsDescriptionPresenter+ViewHolder, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", CustomAbstractDetailsDescriptionPresenter_ViewHolder.class, __md_methods);
	}


	public CustomAbstractDetailsDescriptionPresenter_ViewHolder (android.view.View p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == CustomAbstractDetailsDescriptionPresenter_ViewHolder.class)
			mono.android.TypeManager.Activate ("NetflixComplete.DroidTV.CustomAbstractDetailsDescriptionPresenter+ViewHolder, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Views.View, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public void onLayoutChange (android.view.View p0, int p1, int p2, int p3, int p4, int p5, int p6, int p7, int p8)
	{
		n_onLayoutChange (p0, p1, p2, p3, p4, p5, p6, p7, p8);
	}

	private native void n_onLayoutChange (android.view.View p0, int p1, int p2, int p3, int p4, int p5, int p6, int p7, int p8);

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
