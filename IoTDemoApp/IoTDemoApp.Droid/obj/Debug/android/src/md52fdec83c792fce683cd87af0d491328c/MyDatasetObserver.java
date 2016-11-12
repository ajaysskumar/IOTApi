package md52fdec83c792fce683cd87af0d491328c;


public class MyDatasetObserver
	extends android.database.DataSetObserver
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onChanged:()V:GetOnChangedHandler\n" +
			"";
		mono.android.Runtime.register ("IoTDemoApp.Droid.Activities.MyDatasetObserver, IoTDemoApp.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MyDatasetObserver.class, __md_methods);
	}


	public MyDatasetObserver () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MyDatasetObserver.class)
			mono.android.TypeManager.Activate ("IoTDemoApp.Droid.Activities.MyDatasetObserver, IoTDemoApp.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onChanged ()
	{
		n_onChanged ();
	}

	private native void n_onChanged ();

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
