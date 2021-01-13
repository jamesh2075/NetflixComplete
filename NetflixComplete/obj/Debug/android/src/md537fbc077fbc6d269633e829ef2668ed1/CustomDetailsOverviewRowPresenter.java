package md537fbc077fbc6d269633e829ef2668ed1;


public class CustomDetailsOverviewRowPresenter
	extends android.support.v17.leanback.widget.FullWidthDetailsOverviewRowPresenter
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_createRowViewHolder:(Landroid/view/ViewGroup;)Landroid/support/v17/leanback/widget/RowPresenter$ViewHolder;:GetCreateRowViewHolder_Landroid_view_ViewGroup_Handler\n" +
			"n_initializeRowViewHolder:(Landroid/support/v17/leanback/widget/RowPresenter$ViewHolder;)V:GetInitializeRowViewHolder_Landroid_support_v17_leanback_widget_RowPresenter_ViewHolder_Handler\n" +
			"n_onBindRowViewHolder:(Landroid/support/v17/leanback/widget/RowPresenter$ViewHolder;Ljava/lang/Object;)V:GetOnBindRowViewHolder_Landroid_support_v17_leanback_widget_RowPresenter_ViewHolder_Ljava_lang_Object_Handler\n" +
			"n_onRowViewExpanded:(Landroid/support/v17/leanback/widget/RowPresenter$ViewHolder;Z)V:GetOnRowViewExpanded_Landroid_support_v17_leanback_widget_RowPresenter_ViewHolder_ZHandler\n" +
			"";
		mono.android.Runtime.register ("NetflixComplete.DroidTV.CustomDetailsOverviewRowPresenter, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", CustomDetailsOverviewRowPresenter.class, __md_methods);
	}


	public CustomDetailsOverviewRowPresenter (android.support.v17.leanback.widget.Presenter p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == CustomDetailsOverviewRowPresenter.class)
			mono.android.TypeManager.Activate ("NetflixComplete.DroidTV.CustomDetailsOverviewRowPresenter, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Support.V17.Leanback.Widget.Presenter, Xamarin.Android.Support.v17.Leanback, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}


	public CustomDetailsOverviewRowPresenter (android.support.v17.leanback.widget.Presenter p0, android.support.v17.leanback.widget.DetailsOverviewLogoPresenter p1) throws java.lang.Throwable
	{
		super (p0, p1);
		if (getClass () == CustomDetailsOverviewRowPresenter.class)
			mono.android.TypeManager.Activate ("NetflixComplete.DroidTV.CustomDetailsOverviewRowPresenter, NetflixComplete.DroidTV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Support.V17.Leanback.Widget.Presenter, Xamarin.Android.Support.v17.Leanback, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null:Android.Support.V17.Leanback.Widget.DetailsOverviewLogoPresenter, Xamarin.Android.Support.v17.Leanback, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0, p1 });
	}


	public android.support.v17.leanback.widget.RowPresenter.ViewHolder createRowViewHolder (android.view.ViewGroup p0)
	{
		return n_createRowViewHolder (p0);
	}

	private native android.support.v17.leanback.widget.RowPresenter.ViewHolder n_createRowViewHolder (android.view.ViewGroup p0);


	public void initializeRowViewHolder (android.support.v17.leanback.widget.RowPresenter.ViewHolder p0)
	{
		n_initializeRowViewHolder (p0);
	}

	private native void n_initializeRowViewHolder (android.support.v17.leanback.widget.RowPresenter.ViewHolder p0);


	public void onBindRowViewHolder (android.support.v17.leanback.widget.RowPresenter.ViewHolder p0, java.lang.Object p1)
	{
		n_onBindRowViewHolder (p0, p1);
	}

	private native void n_onBindRowViewHolder (android.support.v17.leanback.widget.RowPresenter.ViewHolder p0, java.lang.Object p1);


	public void onRowViewExpanded (android.support.v17.leanback.widget.RowPresenter.ViewHolder p0, boolean p1)
	{
		n_onRowViewExpanded (p0, p1);
	}

	private native void n_onRowViewExpanded (android.support.v17.leanback.widget.RowPresenter.ViewHolder p0, boolean p1);

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
