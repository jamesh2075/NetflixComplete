using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace NetflixComplete.DroidTV
{
    public class JavaLangObject<T> : Java.Lang.Object
    {
        T obj;

        public JavaLangObject(T obj)
        {
            this.obj = obj;
        }

        public T Object
        {
            get
            {
                return obj;
            }
        }
    }
}