using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using Android.Graphics;
using Android.Graphics.Drawables;

using Android.Support.V17.Leanback.App;
using Android.Support.V17.Leanback.Widget;
using Java.Lang;

using Squareup.Picasso;
using static Android.Graphics.Paint;
using Android.Text;
using Android.Views.Animations;

using NetflixComplete.Portable;

namespace NetflixComplete.DroidTV
{
    public class VideoDetailsFragment : DetailsFragment, IOnActionClickedListener
    {
        private Video selectedVideo;
        private const int BACKGROUND_IMAGE_RECYCLE_INTERVAL = 5000; // 5 seconds
        private const int BACKGROUND_IMAGE_RECYCLE_TIME = 30000; // 30 seconds
        private const int DETAIL_THUMB_WIDTH = 274;
        private const int DETAIL_THUMB_HEIGHT = 274;
        //private const int DETAIL_THUMB_WIDTH = 684;
        //private const int DETAIL_THUMB_HEIGHT = 384;
        //private const int DETAIL_THUMB_WIDTH = 342;
        //private const int DETAIL_THUMB_HEIGHT = 192;
        //private const int DETAIL_THUMB_WIDTH = 684;
        //private const int DETAIL_THUMB_HEIGHT = 384;
        private const int ACTION_PLAY = 1;
        private const int ACTION_ADD_REMOVE_MYLIST = 2;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            var serializedSelectedVideo = Activity.Intent.GetStringExtra("SelectedVideo");
            selectedVideo = Newtonsoft.Json.JsonConvert.DeserializeObject<Video>(serializedSelectedVideo);

            new DetailRowBuilderTask(this).Execute(new JavaLangObject<Video>(selectedVideo));
        }

        public async void OnActionClicked(Android.Support.V17.Leanback.Widget.Action action)
        {
            if (action.Id == ACTION_PLAY)
            {
                Intent netflix = new Intent();
                netflix.SetAction(Intent.ActionView);
                netflix.SetData(Android.Net.Uri.Parse($"nflx://www.netflix.com/watch/{this.selectedVideo.Id}"));
                
                netflix.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                this.Activity.StartActivity(netflix);
                return;

                ////////////////////////////////////////////////////
                // TODO: The code above is not working. Try to get it to deep link into the Netflix app and play the selected video.
                //       Otherwise, comment the code above and allow the code below to run.
                ////////////////////////////////////////////////////

                //Intent intent = new Intent(owner.Activity, typeof(PlayerActivity));
                //intent.PutExtra("SelectedMovie", Newtonsoft.Json.JsonConvert.SerializeObject(owner.selectedVideo));
                //intent.PutExtra(owner.Resources.GetString(Resource.String.should_start), true);
                //owner.StartActivity(intent);

                // The "nflx" Intent Filter is not working on Android TV.
                // Therefore, just add to My List and open the Netflix app.
                // (Note: The "android-app" Intent Filter works fine on Android Mobile OS)
                await NetflixSession.Get().AddVideoToMyListAsync(this.selectedVideo);

                //string videoUrl = $"android-app://com.netflix.mediaclient/http/www.netflix.com/title/{this.selectedVideo.Id}";
                //string videoUrl = $"nflx://www.netflix.com/browse?q=action%3Dsearch%26query%3D{selectedVideo.Title}";
                string videoUrl = $"nflx://www.netflix.com/title/{this.selectedVideo.Id}";
                var url = Android.Net.Uri.Parse(videoUrl);
                var str = url.GetQueryParameter("q");
                Java.Lang.String s = new Java.Lang.String(str);
                var array = s.Split("[?&]");
                Intent intent = new Intent();
                intent.SetData(url);
                this.StartActivity(intent);
            }
            else if (action.Id == ACTION_ADD_REMOVE_MYLIST)
            {
                if (this.selectedVideo.IsInMyList)
                {
                    await NetflixSession.Get().RemoveVideoFromMyListAsync(this.selectedVideo);
                }
                else
                {
                    await NetflixSession.Get().AddVideoToMyListAsync(this.selectedVideo);
                }

                Activity.OnBackPressed();
            }
        }

        private void ModifyMyListAction(Video video, Android.Support.V17.Leanback.Widget.Action action)
        {
            action.Label1 = video.IsInMyList ? "Remove from My List" : "Add to My List";
        }

        private class DetailRowBuilderTask : AsyncTask<JavaLangObject<Video>, Integer, DetailsOverviewRow>
        {
            private readonly VideoDetailsFragment owner;

            public DetailRowBuilderTask(VideoDetailsFragment owner) : base()
            {
                this.owner = owner;
            }

            Bitmap poster = null;
            protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] native_parms)
            {
                // Get the detailed video information.
                // The original Video object is loaded with basic details only.
                JavaLangObject<Video> videoObj = (JavaLangObject<Video>)native_parms[0];
                owner.selectedVideo = NetflixSession.Get().GetDetailedVideoInfo(videoObj.Object);
                videoObj = new JavaLangObject<Video>(owner.selectedVideo);

                DetailsOverviewRow row = new DetailsOverviewRow(videoObj);
                try
                {
                    // the Picasso library helps us dealing with images
                    poster = Picasso.With(owner.Activity)
                            .Load(owner.selectedVideo.CardUrl)
                            .Resize(Utils.dpToPx(DETAIL_THUMB_WIDTH, owner.Activity.ApplicationContext),
                                    Utils.dpToPx(DETAIL_THUMB_HEIGHT, owner.Activity.ApplicationContext))
                            .CenterInside()
                            .Get();
                    row.SetImageBitmap(owner.Activity, poster);
                }
                catch (Java.IO.IOException e)
                {
                    e.PrintStackTrace();
                }

                owner.selectedVideo.BackgroundImageUrls.ForEach(b =>
                {
                    backgroundImages.Add(Picasso.With(owner.Activity)
                            .Load(b)
                            .Get());
                });

                var addRemoveAction = new Android.Support.V17.Leanback.Widget.Action(ACTION_ADD_REMOVE_MYLIST);
                owner.ModifyMyListAction(owner.selectedVideo, addRemoveAction);
                row.ImageScaleUpAllowed = true;

                row.AddAction(addRemoveAction);
                row.AddAction(new Android.Support.V17.Leanback.Widget.Action(ACTION_PLAY, "Play Now"));

                return row;
            }

            List<Bitmap> backgroundImages = new List<Bitmap>();
            protected override void OnPostExecute(Java.Lang.Object result)
            {
                DetailsOverviewRow detailrow = (DetailsOverviewRow)result;
                ClassPresenterSelector ps = new ClassPresenterSelector();
                //FullWidthDetailsOverviewRowPresenter dorPresenter = new FullWidthDetailsOverviewRowPresenter(new DetailsDescriptionPresenter());
                CustomDetailsOverviewRowPresenter dorPresenter = new CustomDetailsOverviewRowPresenter(new DetailsDescriptionPresenter());

                //BitmapDrawable bd = new BitmapDrawable(background);
                //owner.View.Background = bd;
                //owner.Timer = new BackgroundImageTimer(owner, backgroundImages, BACKGROUND_IMAGE_RECYCLE_TIME, BACKGROUND_IMAGE_RECYCLE_INTERVAL);
                //owner.Timer.Start();

                AnimationDrawable animationDrawable = new AnimationDrawable();
                //animationDrawable.SetEnterFadeDuration(500);
                animationDrawable.SetExitFadeDuration(500);
                backgroundImages.ToList().ForEach(b => animationDrawable.AddFrame(new BitmapDrawable(b), BACKGROUND_IMAGE_RECYCLE_INTERVAL));
                owner.View.Background = animationDrawable;
                animationDrawable.Start();

                dorPresenter.OnActionClickedListener = owner;
                ps.AddClassPresenter(Utils.ToJavaClass(typeof(DetailsOverviewRow)), dorPresenter);

                ArrayObjectAdapter adapter = new ArrayObjectAdapter(ps);
                adapter.Add((Java.Lang.Object)detailrow);

                owner.Adapter = adapter;
            }

            protected override DetailsOverviewRow RunInBackground(params JavaLangObject<Video>[] @params)
            {
                return (DetailsOverviewRow)DoInBackground(@params);
            }
        }
    }

    // This class exists for testing and debugging only.
    // If this class is removed, OnPostExecute should create an instance of
    // FullWidthDetailsOverviewRowPresenter instead of this class.
    public class CustomDetailsOverviewRowPresenter : FullWidthDetailsOverviewRowPresenter
    {
        public CustomDetailsOverviewRowPresenter(Presenter presenter) : base(presenter)
        {
        }

        protected override RowPresenter.ViewHolder CreateRowViewHolder(ViewGroup parent)
        {
            return base.CreateRowViewHolder(parent);
        }

        protected override void InitializeRowViewHolder(RowPresenter.ViewHolder vh)
        {
            base.InitializeRowViewHolder(vh);
        }

        protected override void OnBindRowViewHolder(RowPresenter.ViewHolder vh, Java.Lang.Object item)
        {
            base.OnBindRowViewHolder(vh, item);
        }

        protected override void OnRowViewExpanded(RowPresenter.ViewHolder vh, bool expanded)
        {
            base.OnRowViewExpanded(vh, expanded);
        }
    }

    /// <summary>
    /// The DetailsDescriptionPresenter class originally inherited from AbstractDetailsDescriptionPresenter.
    /// However, it now inherits from CustomAbstractDetailsDescriptionPresenter so that it can set
    /// a "Rating" TextView.
    /// </summary>
    public class DetailsDescriptionPresenter : CustomAbstractDetailsDescriptionPresenter
    {
        protected override void OnBindDescription(ViewHolder viewHolder, Java.Lang.Object objectValue)
        {
            JavaLangObject<Video> videoObj = objectValue as JavaLangObject<Video>;
            Video video = videoObj?.Object;
            if (video != null)
            {
                viewHolder.Title.Text = $"{video.Title} ({video.ReleaseYear})";

                viewHolder.Rating.Text = video.Rating;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                if (video.Starring.Count >= 1)
                {
                    sb.Append(" starring ");
                    sb.Append(string.Join(", ", video.Starring.ToArray()));
                }
                viewHolder.Subtitle.Text = $"{video.Type}: {sb.ToString()}";
                viewHolder.Body.Text = video.Synopsis;
            }
        }
    }

    /// <summary>
    /// This implementation copies the AbstractDetailsDescriptionPresenter class
    /// from the Leanback framework. The AbstractDetailsDescriptionPresenter class
    /// does not allow you to add new views to the presenter.
    /// This implementation adds a Rating TextView.
    /// </summary>
    public abstract class CustomAbstractDetailsDescriptionPresenter : Presenter
    {
        public new class ViewHolder : Presenter.ViewHolder, View.IOnLayoutChangeListener
        {
            internal TextView mTitle;
            internal TextView mRating;
            internal TextView mSubtitle;
            internal TextView mBody;
            internal int mTitleMargin;
            internal int mUnderTitleBaselineMargin;
            internal int mUnderSubtitleBaselineMargin;
            internal int mTitleLineSpacing;
            internal int mBodyLineSpacing;
            internal int mBodyMaxLines;
            internal int mBodyMinLines;
            internal FontMetricsInt mTitleFontMetricsInt;
            internal FontMetricsInt mSubtitleFontMetricsInt;
            internal FontMetricsInt mBodyFontMetricsInt;

            public ViewHolder(View view) : base(view)
            {
                mTitle = (TextView)view.FindViewById(Resource.Id.lb_details_description_title);
                mRating = (TextView)view.FindViewById(Resource.Id.lb_details_description_rating);
                mSubtitle = (TextView)view.FindViewById(Resource.Id.lb_details_description_subtitle);
                mBody = (TextView)view.FindViewById(Resource.Id.lb_details_description_body);

                FontMetricsInt titleFontMetricsInt = getFontMetricsInt(mTitle);
                int titleAscent = view.Resources.GetDimensionPixelSize(
                        Resource.Dimension.lb_details_description_title_baseline);
                // Ascent is negative
                mTitleMargin = titleAscent + titleFontMetricsInt.Ascent;

                mUnderTitleBaselineMargin = view.Resources.GetDimensionPixelSize(
                        Resource.Dimension.lb_details_description_under_title_baseline_margin);
                mUnderSubtitleBaselineMargin = view.Resources.GetDimensionPixelSize(
                        Resource.Dimension.lb_details_description_under_subtitle_baseline_margin);

                mTitleLineSpacing = view.Resources.GetDimensionPixelSize(
                        Resource.Dimension.lb_details_description_title_line_spacing);
                mBodyLineSpacing = view.Resources.GetDimensionPixelSize(
                        Resource.Dimension.lb_details_description_body_line_spacing);

                mBodyMaxLines = view.Resources.GetInteger(
                        Resource.Integer.lb_details_description_body_max_lines);
                mBodyMinLines = view.Resources.GetInteger(
                        Resource.Integer.lb_details_description_body_min_lines);

                mTitleFontMetricsInt = getFontMetricsInt(mTitle);
                mSubtitleFontMetricsInt = getFontMetricsInt(mSubtitle);
                mBodyFontMetricsInt = getFontMetricsInt(mBody);

                mTitle.AddOnLayoutChangeListener(this);
            }

            public void OnLayoutChange(View v, int left, int top, int right, int bottom, int oldLeft, int oldTop, int oldRight, int oldBottom)
            {
                mBody.SetMaxLines(mTitle.LineCount > 1 ? mBodyMinLines : mBodyMaxLines);
            }

            public TextView Title
            {
                get
                {
                    return mTitle;
                }
            }

            public TextView Rating
            {
                get
                {
                    return mRating;
                }
            }

            public TextView Subtitle
            {
                get
                {
                    return mSubtitle;
                }
            }

            public TextView Body
            {
                get
                {
                    return mBody;
                }
            }

            private FontMetricsInt getFontMetricsInt(TextView textView)
            {
                Paint paint = new Paint(PaintFlags.AntiAlias);
                paint.TextSize = textView.TextSize;
                paint.SetTypeface(textView.Typeface);
                return paint.GetFontMetricsInt();
            }
        }

        public override Presenter.ViewHolder OnCreateViewHolder(ViewGroup parent)
        {
            View v = LayoutInflater.From(parent.Context)
                .Inflate(Resource.Layout.VideoDetailsDescription, parent, false);
            return new CustomAbstractDetailsDescriptionPresenter.ViewHolder(v);
        }

        public override void OnBindViewHolder(Presenter.ViewHolder viewHolder, Java.Lang.Object item)
        {
            ViewHolder vh = (ViewHolder)viewHolder;
            OnBindDescription(vh, item);

            bool hasTitle = true;
            if (TextUtils.IsEmpty(vh.mTitle.Text))
            {
                vh.mTitle.Visibility = ViewStates.Gone;
                hasTitle = false;
            }
            else
            {
                vh.mTitle.Visibility = ViewStates.Visible;
                vh.mTitle.SetLineSpacing(vh.mTitleLineSpacing - vh.mTitle.LineHeight +
                        vh.mTitle.LineSpacingExtra, vh.mTitle.LineSpacingMultiplier);
            }
            setTopMargin(vh.mTitle, vh.mTitleMargin);

            bool hasSubtitle = true;
            if (TextUtils.IsEmpty(vh.mSubtitle.Text))
            {
                vh.mSubtitle.Visibility = ViewStates.Gone;
                hasSubtitle = false;
            }
            else
            {
                vh.mSubtitle.Visibility = ViewStates.Visible;
                if (hasTitle)
                {
                    setTopMargin(vh.mSubtitle, vh.mUnderTitleBaselineMargin +
                            vh.mSubtitleFontMetricsInt.Ascent - vh.mTitleFontMetricsInt.Descent);

                    setTopMargin(vh.mRating, vh.mUnderTitleBaselineMargin +
                            vh.mSubtitleFontMetricsInt.Ascent - vh.mTitleFontMetricsInt.Descent);
                }
                else
                {
                    setTopMargin(vh.mSubtitle, 0);

                    setTopMargin(vh.mRating, 0);
                }
            }

            if (TextUtils.IsEmpty(vh.mBody.Text))
            {
                vh.mBody.Visibility = ViewStates.Gone;
            }
            else
            {
                vh.mBody.Visibility = ViewStates.Visible;
                vh.mBody.SetLineSpacing(vh.mBodyLineSpacing - vh.mBody.LineHeight +
                        vh.mBody.LineSpacingExtra, vh.mBody.LineSpacingMultiplier);

                if (hasSubtitle)
                {
                    setTopMargin(vh.mBody, vh.mUnderSubtitleBaselineMargin +
                            vh.mBodyFontMetricsInt.Ascent - vh.mSubtitleFontMetricsInt.Descent);
                }
                else if (hasTitle)
                {
                    setTopMargin(vh.mBody, vh.mUnderTitleBaselineMargin +
                            vh.mBodyFontMetricsInt.Ascent - vh.mTitleFontMetricsInt.Descent);
                }
                else
                {
                    setTopMargin(vh.mBody, 0);
                }
            }
        }

        // *
        // * Binds the data from the item referenced in the DetailsOverviewRow to the
        // * ViewHolder.
        // *
        // * @param vh The ViewHolder for this details description view.
        // * @param item The item from the DetailsOverviewRow being presented.
        // 
        protected abstract void OnBindDescription(ViewHolder vh, Java.Lang.Object item);

        public override void OnUnbindViewHolder(Presenter.ViewHolder viewHolder) { }

        private void setTopMargin(TextView textView, int topMargin)
        {
            ViewGroup.MarginLayoutParams lp = (ViewGroup.MarginLayoutParams)textView.LayoutParameters;
            lp.TopMargin = topMargin;
            textView.LayoutParameters = lp;
        }
    }
}