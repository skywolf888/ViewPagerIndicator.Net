//package com.viewpagerindicator;

//import android.content.Context;
//import android.content.res.TypedArray;
//import android.graphics.Canvas;
//import android.graphics.drawable.Drawable;
//import android.view.View;
//import android.widget.LinearLayout;

using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;


namespace Com.ViewPagerIndicator
{
    /**
     * A simple extension of a regular linear layout that supports the divider API
     * of Android 4.0+. The dividers are added adjacent to the children by changing
     * their layout params. If you need to rely on the margins which fall in the
     * same orientation as the layout you should wrap the child in a simple
     * {@link android.widget.FrameLayout} so it can receive the margin.
     */
    class IcsLinearLayout : LinearLayout
    {
        private static int[] LL = new int[] {
        /* 0 */ Android.Resource.Attribute.Divider,
        /* 1 */ Android.Resource.Attribute.ShowDividers,
        /* 2 */ Android.Resource.Attribute.DividerPadding,
    };
        private static int LL_DIVIDER = 0;
        private static int LL_SHOW_DIVIDER = 1;
        private static int LL_DIVIDER_PADDING = 2;

        private Drawable mDivider;
        private int mDividerWidth;
        private int mDividerHeight;
        private ShowDividers mShowDividers;
        private int mDividerPadding;


        public IcsLinearLayout(Context context, int themeAttr)
            : base(context)
        {		
            TypedArray a = context.ObtainStyledAttributes(null, LL, themeAttr, 0);
            setDividerDrawable(a.GetDrawable(IcsLinearLayout.LL_DIVIDER));
            mDividerPadding = a.GetDimensionPixelSize(LL_DIVIDER_PADDING, 0);
            mShowDividers =(ShowDividers) a.GetInteger(LL_SHOW_DIVIDER, (int)ShowDividers.None);
            a.Recycle();
        }

        public void setDividerDrawable(Drawable divider)
        {
            if (divider == mDivider)
            {
                return;
            }
            mDivider = divider;
            if (divider != null)
            {
                mDividerWidth = divider.IntrinsicWidth;
                mDividerHeight = divider.IntrinsicHeight;
            }
            else
            {
                mDividerWidth = 0;
                mDividerHeight = 0;
            }
            SetWillNotDraw(divider == null);
            RequestLayout();
        }

        //@Override
        protected override void MeasureChildWithMargins(View child, int parentWidthMeasureSpec, int widthUsed, int parentHeightMeasureSpec, int heightUsed)
        {
            int index = IndexOfChild(child);
            Android.Widget.Orientation orientation = this.Orientation;
            Android.Widget.LinearLayout.LayoutParams lparams = (Android.Widget.LinearLayout.LayoutParams)child.LayoutParameters;
            if (hasDividerBeforeChildAt(index))
            {
                if (orientation == Android.Widget.Orientation.Vertical)
                {
                    //Account for the divider by pushing everything up
                    lparams.TopMargin = mDividerHeight;
                }
                else
                {
                    //Account for the divider by pushing everything left
                    lparams.LeftMargin = mDividerWidth;
                }
            }

            int count = ChildCount;
            if (index == count - 1)
            {
                if (hasDividerBeforeChildAt(count))
                {
                    if (orientation == Android.Widget.Orientation.Vertical)
                    {
                        lparams.BottomMargin = mDividerHeight;
                    }
                    else
                    {
                        lparams.RightMargin = mDividerWidth;
                    }
                }
            }
            base.MeasureChildWithMargins(child, parentWidthMeasureSpec, widthUsed, parentHeightMeasureSpec, heightUsed);
        }

        //@Override
        protected override void OnDraw(Canvas canvas)
        {
            if (mDivider != null)
            {
                if (this.Orientation == Android.Widget.Orientation.Vertical)
                {
                    drawDividersVertical(canvas);
                }
                else
                {
                    drawDividersHorizontal(canvas);
                }
            }
            base.OnDraw(canvas);
        }

        private void drawDividersVertical(Canvas canvas)
        {
            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
                View child = GetChildAt(i);

                if (child != null && child.Visibility != ViewStates.Gone)
                {
                    if (hasDividerBeforeChildAt(i))
                    {
                        Android.Widget.LinearLayout.LayoutParams lp = (Android.Widget.LinearLayout.LayoutParams)child.LayoutParameters;
                        int top = child.Top - lp.TopMargin/* - mDividerHeight*/;
                        drawHorizontalDivider(canvas, top);
                    }
                }
            }

            if (hasDividerBeforeChildAt(count))
            {
                View child = GetChildAt(count - 1);
                int bottom = 0;
                if (child == null)
                {
                    bottom = Height - PaddingBottom - mDividerHeight;
                }
                else
                {                    
                    //LayoutParams lp = (LayoutParams) child.LayoutParameters;
                    bottom = child.Bottom/* + lp.bottomMargin*/;
                }
                drawHorizontalDivider(canvas, bottom);
            }
        }

        private void drawDividersHorizontal(Canvas canvas)
        {
            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
                View child = GetChildAt(i);

                if (child != null && child.Visibility != ViewStates.Gone)
                {
                    if (hasDividerBeforeChildAt(i))
                    {
                        Android.Widget.LinearLayout.LayoutParams lp = (LayoutParams)child.LayoutParameters;
                        int left = child.Left - lp.LeftMargin/* - mDividerWidth*/;
                        drawVerticalDivider(canvas, left);
                    }
                }
            }

            if (hasDividerBeforeChildAt(count))
            {
                View child = GetChildAt(count - 1);
                int right = 0;
                if (child == null)
                {
                    right = Width - PaddingRight - mDividerWidth;
                }
                else
                {
                    // LayoutParams lp = (LayoutParams) child.getLayoutParams();
                    right = child.Right/* + lp.rightMargin*/;
                }
                drawVerticalDivider(canvas, right);
            }
        }

        private void drawHorizontalDivider(Canvas canvas, int top)
        {
            mDivider.SetBounds(PaddingLeft + mDividerPadding, top,
                    Width - PaddingRight - mDividerPadding, top + mDividerHeight);
            mDivider.Draw(canvas);
        }

        private void drawVerticalDivider(Canvas canvas, int left)
        {
            mDivider.SetBounds(left, PaddingTop + mDividerPadding,
                    left + mDividerWidth, Height - PaddingBottom - mDividerPadding);
            mDivider.Draw(canvas);
        }

        private bool hasDividerBeforeChildAt(int childIndex)
        {
            if (childIndex == 0 || childIndex == ChildCount)
            {
                return false;
            }
            if ((mShowDividers & ShowDividers.Middle) != 0)
            {
                bool hasVisibleViewBefore = false;
                for (int i = childIndex - 1; i >= 0; i--)
                {
                    if (GetChildAt(i).Visibility != ViewStates.Gone)
                    {
                        hasVisibleViewBefore = true;
                        break;
                    }
                }

                return hasVisibleViewBefore;
            }
            return false;
        }
    }
}