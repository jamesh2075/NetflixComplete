using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V17.Leanback.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace NetflixComplete.DroidTV
{
    public class CategoriesFragment : ListFragment
    {
        /// <summary>
        /// Browse to the category that the user tapped.
        /// If the user tapped a category that is currently being viewed,
        /// simply do nothing.
        /// </summary>
        private long currentCategoryId = 0;
        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var item = l.GetItemAtPosition(position);
            Category category = item as Category;

            // If the user tapped the category currently being viewed, simply return.
            if (currentCategoryId == category.ID)
                return;

            currentCategoryId = category.ID;

            base.OnListItemClick(l, v, position, id);

            Fragment fragment = position == 0 ? (Fragment)new AboutFragment() { Arguments = new Bundle() } : new VideosFragment() { Arguments = new Bundle() };
            fragment.Arguments.PutLong("CategoryId", category.ID);
            fragment.Arguments.PutString("CategoryName", category.Name);

            ReplaceFrameLayoutWith(fragment);
        }

        private void ReplaceFrameLayoutWith(Fragment fragment)
        {
            var trx = this.FragmentManager.BeginTransaction();
            trx.Replace(Resource.Id.movies, fragment);
            trx.SetTransition(FragmentTransit.FragmentFade);
            trx.Commit();
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            ArrayAdapter adapter = new ArrayAdapter(Activity, Android.Resource.Layout.SimpleSelectableListItem);
            this.ListAdapter = adapter;

            Category aboutAppCategory = new Category { ID = 0, Name = "About this app" };
            adapter.Add(aboutAppCategory);

            System.IO.Stream input = Activity.Assets.Open("Categories.txt");
            using (System.IO.StreamReader reader = new System.IO.StreamReader(input))
            {
                while (!reader.EndOfStream)
                {
                    string categoryLine = reader.ReadLine();
                    var categoryArray = categoryLine.Split(':');
                    string categoryName = categoryArray[0];
                    long categoryNumber = long.Parse(categoryArray[1]);

                    Category category = new Category { ID = categoryNumber, Name = categoryName };
                    adapter.Add(category);
                }
            }

            // Show the About this App fragment upon startup.
            ReplaceFrameLayoutWith(new AboutFragment());
        }
    }

    class Category : Java.Lang.Object
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }

    public class AboutFragment : Android.Support.V17.Leanback.App.DetailsFragment
    {
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            AboutApp app = new AboutApp();
            Android.Support.V17.Leanback.Widget.DetailsOverviewRow row = new Android.Support.V17.Leanback.Widget.DetailsOverviewRow(app);
            var drawable = Resources.GetDrawable(Resource.Drawable.Login);
            //var bd = (Android.Graphics.Drawables.BitmapDrawable)drawable;
            //row.SetImageBitmap(Activity, bd.Bitmap);
            //row.ImageDrawable = drawable;
            this.View.Background = drawable;

            CustomDetailsOverviewRowPresenter ps = new CustomDetailsOverviewRowPresenter(new DetailsDescriptionPresenter());
            var adapter = new Android.Support.V17.Leanback.Widget.ArrayObjectAdapter(ps);
            adapter.Add(row);
            this.Adapter = adapter;
        }

        class AboutApp : Java.Lang.Object
        {
            public string Company { get; } = "BLUEVISION, LLC";
            public string Developer { get; } = "James Henry";
            public string EmailAddress { get; } = "netflixcomplete@bluevisionsoftware.com";
        }

        class DetailsDescriptionPresenter : Android.Support.V17.Leanback.Widget.AbstractDetailsDescriptionPresenter
        {
            protected override void OnBindDescription(ViewHolder viewHolder, Java.Lang.Object objectValue)
            {
                AboutApp app = (AboutApp)objectValue;
                viewHolder.Title.Text = "About this app";
                viewHolder.Subtitle.Text = $"Created by {app.Developer} {app.Company} {app.EmailAddress}";
                viewHolder.Body.Text = viewHolder.View.Resources.GetString(Resource.String.terms_of_use);
            }
        }
    }
}