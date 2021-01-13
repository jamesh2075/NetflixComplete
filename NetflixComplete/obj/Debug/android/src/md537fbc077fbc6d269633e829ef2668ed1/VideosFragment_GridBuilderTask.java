package md537fbc077fbc6d269633e829ef2668ed1;


public class VideosFragment_GridBuilderTask
	extends android.os.AsyncTask
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_doInBackground:([Ljava/lang/Object;)Ljava/lang/Object;:GetDoInBackground_arrayLjava_lang_Object_Handler\n" +
			"n_onPostExecute:(Ljava/lang/Object;)V:GetOnPostExecute_Ljava_lang_Object_Handler\n" +
			"";
		mono.android.Runtime.register ("NetflixComplete.DroidTV.VideosFragment+GridBuilderTask, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", VideosFragment_GridBuilderTask.class, __md_methods);
	}


	public VideosFragment_GridBuilderTask () throws java.lang.Throwable
	{
		super ();
		if (getClass () == VideosFragment_GridBuilderTask.class)
			mono.android.TypeManager.Activate ("NetflixComplete.DroidTV.VideosFragment+GridBuilderTask, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public VideosFragment_GridBuilderTask (md537fbc077fbc6d269633e829ef2668ed1.VideosFragment p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == VideosFragment_GridBuilderTask.class)
			mono.android.TypeManager.Activate ("NetflixComplete.DroidTV.VideosFragment+GridBuilderTask, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "NetflixComplete.DroidTV.VideosFragment, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}


	public java.lang.Object doInBackground (java.lang.Object[] p0)
	{
		return n_doInBackground (p0);
	}

	private native java.lang.Object n_doInBackground (java.lang.Object[] p0);


	public void onPostExecute (java.lang.Object p0)
	{
		n_onPostExecute (p0);
	}

	private native void n_onPostExecute (java.lang.Object p0);

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
