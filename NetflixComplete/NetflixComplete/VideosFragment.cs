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

using Squareup.Picasso;

using NetflixComplete.Portable;

namespace NetflixComplete.DroidTV
{
    public class VideosFragment : VerticalGridFragment
    {
        private const int NUMBER_OF_COLUMNS = 4;

        LinearLayout pFrame = null;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = base.OnCreateView(inflater, container, savedInstanceState);

            var root = view.FindViewById<FrameLayout>(Resource.Id.browse_grid_dock);

            pFrame = new LinearLayout(Activity);
            pFrame.Orientation = Orientation.Vertical;
            pFrame.Visibility = ViewStates.Gone;
            pFrame.SetGravity(GravityFlags.Center);

            ProgressBar progress = new ProgressBar(Activity, null, Android.Resource.Attribute.ProgressBarStyleLarge);
            pFrame.AddView(progress, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));

            root.AddView(pFrame);

            return view;
        }

        public LinearLayout ProgressContainer
        {
            get
            {
                return pFrame;
            }
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            pFrame.Visibility = ViewStates.Visible;

            long categoryId = Arguments.GetLong("CategoryId");
            string categoryName = Arguments.GetString("CategoryName");

            Category category = new Category { ID = categoryId, Name = categoryName };

            new GridBuilderTask(this).Execute(category);

            //var videos = NetflixApi.GetVideosByCategory(categoryId);

            //pFrame.Visibility = ViewStates.Gone;

            //int count = videos.Count;
            //var plural = count != 1 ? "s" : "";
            //Title = $"{categoryName} ({count} video{plural})";

            //var cardPresenter = new CardPresenter();
            //ArrayObjectAdapter adapter = new ArrayObjectAdapter(cardPresenter);
            //adapter.AddAll(0, videos);
            //this.Adapter = adapter;

            base.OnActivityCreated(savedInstanceState);


        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            VerticalGridPresenter presenter = new VerticalGridPresenter();
            presenter.NumberOfColumns = NUMBER_OF_COLUMNS;
            this.GridPresenter = presenter;

            base.OnCreate(savedInstanceState);

            // Create your fragment here
            ItemViewClicked += (sender, args) =>
            {
                var video = ((JavaLangObject<Video>)args.Item).Object;
                var serializedVideo = Newtonsoft.Json.JsonConvert.SerializeObject(video);
                Intent intent = new Intent(this.Activity, typeof(VideoDetailsActivity));
                intent.PutExtra("SelectedVideo", serializedVideo);
                StartActivity(intent);
            };
        }

        private class GridBuilderTask : AsyncTask<Category, Java.Lang.Integer, VideoListHolder>
        {
            private readonly VideosFragment owner;

            public GridBuilderTask(VideosFragment owner) : base()
            {
                this.owner = owner;
            }

            protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] native_parms)
            {
                Category category = (Category)native_parms[0];

                var videos = NetflixSession.Get().GetVideosByCategory(category.ID);

                VideoListHolder holder = new VideoListHolder(category, videos);
                return holder;
            }

            protected override void OnPostExecute(Java.Lang.Object result)
            {
                VideoListHolder holder = (VideoListHolder)result;
                List<Video> videos = holder.Videos;

                int count = videos.Count;
                var plural = count != 1 ? "s" : "";
                owner.Title = $"{holder.Category.Name} ({count} video{plural})";

                var cardPresenter = new CardPresenter();
                ArrayObjectAdapter adapter = new ArrayObjectAdapter(cardPresenter);
                var videoObjs = videos.ConvertAll<JavaLangObject<Video>>((v) =>
                {
                    return new JavaLangObject<Video>(v);
                });
                adapter.AddAll(0, videoObjs);

                owner.Adapter = adapter;

                owner.ProgressContainer.Visibility = ViewStates.Gone;
            }

            protected override VideoListHolder RunInBackground(params Category[] @params)
            {
                return (VideoListHolder)DoInBackground(@params);
            }
        }

        internal class VideoListHolder : Java.Lang.Object
        {
            private List<Video> videos;
            private Category category;

            public VideoListHolder(Category category, List<Video> videos)
            {
                this.category = category;
                this.videos = videos;
            }
            
            public Category Category { get { return this.category; } }
            public List<Video> Videos { get { return this.videos; } }
        }
    }

    public class CardPresenter : Presenter
    {
        private const string TAG = "CardPresenter";

        protected static Context mContext
        {
            get;
            private set;
        }

        public static readonly int CARD_WIDTH = 250;
        public static readonly int CARD_HEIGHT = 250;


        public override ViewHolder OnCreateViewHolder(ViewGroup parent)
        {
            mContext = parent.Context;

            // If an exception is thrown when instantiating ImageCardView, 
            // ensure that the AndroidManifest.xml file has the following:
            // <application android: theme = "@style/Theme.Leanback" />
            var cardView = new ImageCardView(mContext);
            cardView.Focusable = true;
            cardView.FocusableInTouchMode = true;
            cardView.SetBackgroundColor(mContext.Resources.GetColor(Resource.Color.lb_basic_card_info_bg_color));
            return new CustomViewHolder(cardView);
        }

        public override void OnBindViewHolder(Presenter.ViewHolder viewHolder, Java.Lang.Object item)
        {
            Video video = ((JavaLangObject<Video>)item).Object;
            ((CustomViewHolder)viewHolder).movie = video;

            Java.Net.URI uri = new Java.Net.URI(video.CardUrl);
            if (uri != null)
            {
                var holder = (CustomViewHolder)viewHolder;
                var card = holder.cardView;
                card.TitleText = video.Title;
                card.ContentText = video.Type;
                card.ContentDescription = "This is a test";
                card.CardType = ImageCardView.CardTypeInfoUnderWithExtra;
                card.ExtraVisibility = ImageCardView.CardRegionVisibleSelected;
                card.SetMainImageDimensions(CARD_WIDTH, CARD_HEIGHT);
                holder.UpdateCardViewImage(uri);
            }
        }

        public override void OnUnbindViewHolder(Presenter.ViewHolder viewHolder)
        {
        }
    }

    public class PicassoImageCardViewTarget : Java.Lang.Object, ITarget
    {
        private ImageCardView mImageCardView;

        public PicassoImageCardViewTarget(ImageCardView imageCardView)
        {
            mImageCardView = imageCardView;
        }

        public void OnBitmapLoaded(Bitmap bitmap, Picasso.LoadedFrom loadedFrom)
        {
            Drawable bitmapDrawable = new BitmapDrawable(mImageCardView.Context.Resources, bitmap); //TODO get proper context
            mImageCardView.MainImage = bitmapDrawable;
        }

        public void OnBitmapFailed(Drawable drawable)
        {
            mImageCardView.MainImage = drawable;
        }

        public void OnPrepareLoad(Drawable drawable)
        {
            // Do nothing, default_background manager has its own transitions
        }

    }

    public class CustomViewHolder : Android.Support.V17.Leanback.Widget.Presenter.ViewHolder
    {
        private readonly Context mContext;

        public Video movie
        {
            get;
            set;
        }

        public ImageCardView cardView
        {
            get;
            private set;
        }

        public string TitleText
        {
            set
            {
                cardView.TitleText = value;
            }
            get
            {
                return cardView.TitleText;
            }
        }

        public string ContentText
        {
            set
            {
                cardView.ContentText = value;
            }
            get
            {
                return cardView.ContentText;
            }
        }

        private Drawable mDefaultCardImage;
        private PicassoImageCardViewTarget mImageCardViewTarget;

        public CustomViewHolder(View view) : base(view)
        {
            cardView = (ImageCardView)view;
            mContext = view.Context;
            mImageCardViewTarget = new PicassoImageCardViewTarget(cardView);
            mDefaultCardImage = mContext.Resources.GetDrawable(Resource.Drawable.movie);
        }

        public void UpdateCardViewImage(Java.Net.URI uri) //FIXME prob this version, maybe don't work with either
        {
            Picasso.With(mContext)
                .Load(uri.ToString())
                .Resize(Utils.dpToPx(CardPresenter.CARD_WIDTH, mContext),
                Utils.dpToPx(CardPresenter.CARD_HEIGHT, mContext))
                .Error(mDefaultCardImage)
                .Into(mImageCardViewTarget);
        }
    }

    public static class Utils
    {
        public static int dpToPx(int dp, Context ctx)
        {
            float density = ctx.Resources.DisplayMetrics.Density;
            return (int)Math.Round((float)dp * density);
        }

        public static Java.Lang.Class ToJavaClass(Type t)
        {
            return Java.Lang.Class.FromType(t);
        }

        public static void ShowErrorDialog(Context context, String errorString)
        {
            new AlertDialog.Builder(context).SetTitle(Resource.String.error)
                .SetMessage(errorString)
                .SetPositiveButton(Resource.String.ok, (object s, DialogClickEventArgs e) => {
                    if (s is IDialogInterface)
                    {
                        ((IDialogInterface)s).Cancel(); //TODO test
                        Log.Info("Dialog", "Canceling"); //TODO remove
                    }
                }).Create().Show();
        }
    }
}