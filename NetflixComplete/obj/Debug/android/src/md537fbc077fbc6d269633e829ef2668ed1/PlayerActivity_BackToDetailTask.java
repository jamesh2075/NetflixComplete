package md537fbc077fbc6d269633e829ef2668ed1;


public class PlayerActivity_BackToDetailTask
	extends java.util.TimerTask
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_run:()V:GetRunHandler\n" +
			"";
		mono.android.Runtime.register ("NetflixComplete.DroidTV.PlayerActivity+BackToDetailTask, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", PlayerActivity_BackToDetailTask.class, __md_methods);
	}


	public PlayerActivity_BackToDetailTask () throws java.lang.Throwable
	{
		super ();
		if (getClass () == PlayerActivity_BackToDetailTask.class)
			mono.android.TypeManager.Activate ("NetflixComplete.DroidTV.PlayerActivity+BackToDetailTask, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public PlayerActivity_BackToDetailTask (md537fbc077fbc6d269633e829ef2668ed1.PlayerActivity p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == PlayerActivity_BackToDetailTask.class)
			mono.android.TypeManager.Activate ("NetflixComplete.DroidTV.PlayerActivity+BackToDetailTask, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "NetflixComplete.DroidTV.PlayerActivity, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}


	public void run ()
	{
		n_run ();
	}

	private native void n_run ();

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
