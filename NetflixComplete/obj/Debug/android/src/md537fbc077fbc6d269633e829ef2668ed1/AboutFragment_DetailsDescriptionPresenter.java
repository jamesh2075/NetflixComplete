package md537fbc077fbc6d269633e829ef2668ed1;


public class AboutFragment_DetailsDescriptionPresenter
	extends android.support.v17.leanback.widget.AbstractDetailsDescriptionPresenter
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onBindDescription:(Landroid/support/v17/leanback/widget/AbstractDetailsDescriptionPresenter$ViewHolder;Ljava/lang/Object;)V:GetOnBindDescription_Landroid_support_v17_leanback_widget_AbstractDetailsDescriptionPresenter_ViewHolder_Ljava_lang_Object_Handler\n" +
			"";
		mono.android.Runtime.register ("NetflixComplete.DroidTV.AboutFragment+DetailsDescriptionPresenter, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", AboutFragment_DetailsDescriptionPresenter.class, __md_methods);
	}


	public AboutFragment_DetailsDescriptionPresenter () throws java.lang.Throwable
	{
		super ();
		if (getClass () == AboutFragment_DetailsDescriptionPresenter.class)
			mono.android.TypeManager.Activate ("NetflixComplete.DroidTV.AboutFragment+DetailsDescriptionPresenter, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onBindDescription (android.support.v17.leanback.widget.AbstractDetailsDescriptionPresenter.ViewHolder p0, java.lang.Object p1)
	{
		n_onBindDescription (p0, p1);
	}

	private native void n_onBindDescription (android.support.v17.leanback.widget.AbstractDetailsDescriptionPresenter.ViewHolder p0, java.lang.Object p1);

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
