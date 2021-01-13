package md537fbc077fbc6d269633e829ef2668ed1;


public class VideoDetailsFragment
	extends android.support.v17.leanback.app.DetailsFragment
	implements
		mono.android.IGCUserPeer,
		android.support.v17.leanback.widget.OnActionClickedListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onActionClicked:(Landroid/support/v17/leanback/widget/Action;)V:GetOnActionClicked_Landroid_support_v17_leanback_widget_Action_Handler:Android.Support.V17.Leanback.Widget.IOnActionClickedListenerInvoker, Xamarin.Android.Support.v17.Leanback\n" +
			"";
		mono.android.Runtime.register ("NetflixComplete.DroidTV.VideoDetailsFragment, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", VideoDetailsFragment.class, __md_methods);
	}


	public VideoDetailsFragment () throws java.lang.Throwable
	{
		super ();
		if (getClass () == VideoDetailsFragment.class)
			mono.android.TypeManager.Activate ("NetflixComplete.DroidTV.VideoDetailsFragment, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void onActionClicked (android.support.v17.leanback.widget.Action p0)
	{
		n_onActionClicked (p0);
	}

	private native void n_onActionClicked (android.support.v17.leanback.widget.Action p0);

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
