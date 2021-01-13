package md537fbc077fbc6d269633e829ef2668ed1;


public class DetailsDescriptionPresenter
	extends md537fbc077fbc6d269633e829ef2668ed1.CustomAbstractDetailsDescriptionPresenter
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("NetflixComplete.DroidTV.DetailsDescriptionPresenter, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", DetailsDescriptionPresenter.class, __md_methods);
	}


	public DetailsDescriptionPresenter () throws java.lang.Throwable
	{
		super ();
		if (getClass () == DetailsDescriptionPresenter.class)
			mono.android.TypeManager.Activate ("NetflixComplete.DroidTV.DetailsDescriptionPresenter, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

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
