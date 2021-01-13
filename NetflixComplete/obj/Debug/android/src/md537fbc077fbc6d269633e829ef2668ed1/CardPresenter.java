package md537fbc077fbc6d269633e829ef2668ed1;


public class CardPresenter
	extends android.support.v17.leanback.widget.Presenter
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreateViewHolder:(Landroid/view/ViewGroup;)Landroid/support/v17/leanback/widget/Presenter$ViewHolder;:GetOnCreateViewHolder_Landroid_view_ViewGroup_Handler\n" +
			"n_onBindViewHolder:(Landroid/support/v17/leanback/widget/Presenter$ViewHolder;Ljava/lang/Object;)V:GetOnBindViewHolder_Landroid_support_v17_leanback_widget_Presenter_ViewHolder_Ljava_lang_Object_Handler\n" +
			"n_onUnbindViewHolder:(Landroid/support/v17/leanback/widget/Presenter$ViewHolder;)V:GetOnUnbindViewHolder_Landroid_support_v17_leanback_widget_Presenter_ViewHolder_Handler\n" +
			"";
		mono.android.Runtime.register ("NetflixComplete.DroidTV.CardPresenter, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", CardPresenter.class, __md_methods);
	}


	public CardPresenter () throws java.lang.Throwable
	{
		super ();
		if (getClass () == CardPresenter.class)
			mono.android.TypeManager.Activate ("NetflixComplete.DroidTV.CardPresenter, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public android.support.v17.leanback.widget.Presenter.ViewHolder onCreateViewHolder (android.view.ViewGroup p0)
	{
		return n_onCreateViewHolder (p0);
	}

	private native android.support.v17.leanback.widget.Presenter.ViewHolder n_onCreateViewHolder (android.view.ViewGroup p0);


	public void onBindViewHolder (android.support.v17.leanback.widget.Presenter.ViewHolder p0, java.lang.Object p1)
	{
		n_onBindViewHolder (p0, p1);
	}

	private native void n_onBindViewHolder (android.support.v17.leanback.widget.Presenter.ViewHolder p0, java.lang.Object p1);


	public void onUnbindViewHolder (android.support.v17.leanback.widget.Presenter.ViewHolder p0)
	{
		n_onUnbindViewHolder (p0);
	}

	private native void n_onUnbindViewHolder (android.support.v17.leanback.widget.Presenter.ViewHolder p0);

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
