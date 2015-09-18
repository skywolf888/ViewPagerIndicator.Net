/*
 * Copyright (C) 2011 Jake Wharton
 * Copyright (C) 2011 Patrik Akerfeldt
 * Copyright (C) 2011 Francisco Figueiredo Jr.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
//package com.viewpagerindicator;

//import android.content.Context;
//import android.content.res.Resources;
//import android.content.res.TypedArray;
//import android.graphics.Canvas;
//import android.graphics.Paint;
//import android.graphics.Path;
//import android.graphics.Rect;
//import android.graphics.Typeface;
//import android.graphics.drawable.Drawable;
//import android.os.Parcel;
//import android.os.Parcelable;
//import android.support.v4.view.MotionEventCompat;
//import android.support.v4.view.ViewConfigurationCompat;
//import android.support.v4.view.ViewPager;
//import android.util.AttributeSet;
//import android.view.MotionEvent;
//import android.view.View;
//import android.view.ViewConfiguration;

//import java.util.ArrayList;


using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using System;
using System.Collections.Generic;
using R=ViewPagerIndicator.Net.Resource;




namespace Com.ViewPagerIndicator
{

    /**
     * A TitlePageIndicator is a PageIndicator which displays the title of left view
     * (if exist), the title of the current select view (centered) and the title of
     * the right view (if exist). When the user scrolls the ViewPager then titles are
     * also scrolled.
     */

    public enum IndicatorStyle
    {
        None = 0, Triangle = 1, Underline = 2
        //None(0), Triangle(1), Underline(2);

        //public  int value;

        //private IndicatorStyle(int value) {
        //    this.value = value;
        //}

        //public static IndicatorStyle fromValue(int value) {
        //    for (IndicatorStyle style : IndicatorStyle.values()) {
        //        if (style.value == value) {
        //            return style;
        //        }
        //    }
        //    return null;
        //}
    }

    public enum LinePosition
    {
        Bottom = 0, Top = 1

        //Bottom(0), Top(1);

        //public  int value;

        //private LinePosition(int value) {
        //    this.value = value;
        //}

        //public static LinePosition fromValue(int value) {
        //    for (LinePosition position : LinePosition.values()) {
        //        if (position.value == value) {
        //            return position;
        //        }
        //    }
        //    return null;
        //}
    }

    /**
         * Interface for a callback when the center item has been clicked.
         */
    public interface OnCenterItemClickListener
    {
        /**
         * Callback when the center item has been clicked.
         *
         * @param position Position of the current center item.
         */
        void onCenterItemClick(int position);
    }

    public class TitlePageIndicator : View, IPageIndicator
    {
        /**
         * Percentage indicating what percentage of the screen width away from
         * center should the underline be fully faded. A value of 0.25 means that
         * halfway between the center of the screen and an edge.
         */
        private static float SELECTION_FADE_PERCENTAGE = 0.25f;

        /**
         * Percentage indicating what percentage of the screen width away from
         * center should the selected text bold turn off. A value of 0.05 means
         * that 10% between the center and an edge.
         */
        private static float BOLD_FADE_PERCENTAGE = 0.05f;

        /**
         * Title text used when no title is provided by the adapter.
         */
        private const string EMPTY_TITLE = "";

        


        private ViewPager mViewPager;
        private ViewPager.IOnPageChangeListener mListener;
        private int mCurrentPage = -1;
        private float mPageOffset;
        private int mScrollState;
        private Paint mPaintText = new Paint();
        private bool mBoldText;
        private Color mColorText;
        private Color mColorSelected;
        private Path mPath = new Path();
        private Rect mBounds = new Rect();
        private Paint mPaintFooterLine = new Paint();
        private IndicatorStyle mFooterIndicatorStyle;
        private LinePosition mLinePosition;
        private Paint mPaintFooterIndicator = new Paint();
        private float mFooterIndicatorHeight;
        private float mFooterIndicatorUnderlinePadding;
        private float mFooterPadding;
        private float mTitlePadding;
        private float mTopPadding;
        /** Left and right side padding for not active view titles. */
        private float mClipPadding;
        private float mFooterLineHeight;

        private static int INVALID_POINTER = -1;

        private int mTouchSlop;
        private float mLastMotionX = -1;
        private int mActivePointerId = INVALID_POINTER;
        private bool mIsDragging;

        private OnCenterItemClickListener mCenterItemClickListener;


        public TitlePageIndicator(Context context)
            : this(context, null)
        {

        }

        public TitlePageIndicator(Context context, IAttributeSet attrs)
            : this(context, attrs, R.Attribute.vpiTitlePageIndicatorStyle)
        {

        }

        public TitlePageIndicator(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {

            if (IsInEditMode) return;

            //Load defaults from resources
            Android.Content.Res.Resources res = this.Resources;
            Color defaultFooterColor = res.GetColor(R.Color.default_title_indicator_footer_color);
            float defaultFooterLineHeight = res.GetDimension(R.Dimension.default_title_indicator_footer_line_height);
            int defaultFooterIndicatorStyle = res.GetInteger(R.Integer.default_title_indicator_footer_indicator_style);
            float defaultFooterIndicatorHeight = res.GetDimension(R.Dimension.default_title_indicator_footer_indicator_height);
            float defaultFooterIndicatorUnderlinePadding = res.GetDimension(R.Dimension.default_title_indicator_footer_indicator_underline_padding);
            float defaultFooterPadding = res.GetDimension(R.Dimension.default_title_indicator_footer_padding);
            int defaultLinePosition = res.GetInteger(R.Integer.default_title_indicator_line_position);
            Color defaultSelectedColor = res.GetColor(R.Color.default_title_indicator_selected_color);
            bool defaultSelectedBold = res.GetBoolean(R.Boolean.default_title_indicator_selected_bold);
            Color defaultTextColor = res.GetColor(R.Color.default_title_indicator_text_color);
            float defaultTextSize = res.GetDimension(R.Dimension.default_title_indicator_text_size);
            float defaultTitlePadding = res.GetDimension(R.Dimension.default_title_indicator_title_padding);
            float defaultClipPadding = res.GetDimension(R.Dimension.default_title_indicator_clip_padding);
            float defaultTopPadding = res.GetDimension(R.Dimension.default_title_indicator_top_padding);

            //Retrieve styles attributes
            TypedArray a = context.ObtainStyledAttributes(attrs, R.Styleable.TitlePageIndicator, defStyle, 0);

            //Retrieve the colors to be used for this view and apply them.
            mFooterLineHeight = a.GetDimension(R.Styleable.TitlePageIndicator_footerLineHeight, defaultFooterLineHeight);
            mFooterIndicatorStyle = (IndicatorStyle)(a.GetInteger(R.Styleable.TitlePageIndicator_footerIndicatorStyle, defaultFooterIndicatorStyle));
            mFooterIndicatorHeight = a.GetDimension(R.Styleable.TitlePageIndicator_footerIndicatorHeight, defaultFooterIndicatorHeight);
            mFooterIndicatorUnderlinePadding = a.GetDimension(R.Styleable.TitlePageIndicator_footerIndicatorUnderlinePadding, defaultFooterIndicatorUnderlinePadding);
            mFooterPadding = a.GetDimension(R.Styleable.TitlePageIndicator_footerPadding, defaultFooterPadding);
            mLinePosition = (LinePosition)(a.GetInteger(R.Styleable.TitlePageIndicator_linePosition, defaultLinePosition));
            mTopPadding = a.GetDimension(R.Styleable.TitlePageIndicator_topPadding, defaultTopPadding);
            mTitlePadding = a.GetDimension(R.Styleable.TitlePageIndicator_titlePadding, defaultTitlePadding);
            mClipPadding = a.GetDimension(R.Styleable.TitlePageIndicator_clipPadding, defaultClipPadding);
            mColorSelected = a.GetColor(R.Styleable.TitlePageIndicator_selectedColor, defaultSelectedColor);
            mColorText = a.GetColor(R.Styleable.TitlePageIndicator_android_textColor, defaultTextColor);
            mBoldText = a.GetBoolean(R.Styleable.TitlePageIndicator_selectedBold, defaultSelectedBold);

            float textSize = a.GetDimension(R.Styleable.TitlePageIndicator_android_textSize, defaultTextSize);
            Color footerColor = a.GetColor(R.Styleable.TitlePageIndicator_footerColor, defaultFooterColor);
            mPaintText.TextSize = textSize;
            mPaintText.AntiAlias = true;
            mPaintFooterLine.SetStyle(Paint.Style.FillAndStroke);
            mPaintFooterLine.StrokeWidth = mFooterLineHeight;
            mPaintFooterLine.Color = footerColor;
            mPaintFooterIndicator.SetStyle(Paint.Style.FillAndStroke);
            mPaintFooterIndicator.Color = footerColor;

            Drawable background = a.GetDrawable(R.Styleable.TitlePageIndicator_android_background);
            if (background != null)
            {
                SetBackgroundDrawable(background);
            }

            a.Recycle();

            ViewConfiguration configuration = ViewConfiguration.Get(context);
            mTouchSlop = ViewConfigurationCompat.GetScaledPagingTouchSlop(configuration);
        }


        public int getFooterColor()
        {
            return mPaintFooterLine.Color;
        }

        public void setFooterColor(Color footerColor)
        {
            mPaintFooterLine.Color = footerColor;
            mPaintFooterIndicator.Color = footerColor;
            Invalidate();
        }

        public float getFooterLineHeight()
        {
            return mFooterLineHeight;
        }

        public void setFooterLineHeight(float footerLineHeight)
        {
            mFooterLineHeight = footerLineHeight;
            mPaintFooterLine.StrokeWidth = mFooterLineHeight;
            Invalidate();
        }

        public float getFooterIndicatorHeight()
        {
            return mFooterIndicatorHeight;
        }

        public void setFooterIndicatorHeight(float footerTriangleHeight)
        {
            mFooterIndicatorHeight = footerTriangleHeight;
            Invalidate();
        }

        public float getFooterIndicatorPadding()
        {
            return mFooterPadding;
        }

        public void setFooterIndicatorPadding(float footerIndicatorPadding)
        {
            mFooterPadding = footerIndicatorPadding;
            Invalidate();
        }

        public IndicatorStyle getFooterIndicatorStyle()
        {
            return mFooterIndicatorStyle;
        }

        public void setFooterIndicatorStyle(IndicatorStyle indicatorStyle)
        {
            mFooterIndicatorStyle = indicatorStyle;
            Invalidate();
        }

        public LinePosition getLinePosition()
        {
            return mLinePosition;
        }

        public void setLinePosition(LinePosition linePosition)
        {
            mLinePosition = linePosition;
            Invalidate();
        }

        public int getSelectedColor()
        {
            return mColorSelected;
        }

        public void setSelectedColor(Color selectedColor)
        {
            mColorSelected = selectedColor;
            Invalidate();
        }

        public bool isSelectedBold()
        {
            return mBoldText;
        }

        public void setSelectedBold(bool selectedBold)
        {
            mBoldText = selectedBold;
            Invalidate();
        }

        public int getTextColor()
        {
            return mColorText;
        }

        public void setTextColor(Color textColor)
        {
            mPaintText.Color = textColor;
            mColorText = textColor;
            Invalidate();
        }

        public float getTextSize()
        {
            return mPaintText.TextSize;
        }

        public void setTextSize(float textSize)
        {
            mPaintText.TextSize = textSize;
            Invalidate();
        }

        public float getTitlePadding()
        {
            return this.mTitlePadding;
        }

        public void setTitlePadding(float titlePadding)
        {
            mTitlePadding = titlePadding;
            Invalidate();
        }

        public float getTopPadding()
        {
            return this.mTopPadding;
        }

        public void setTopPadding(float topPadding)
        {
            mTopPadding = topPadding;
            Invalidate();
        }

        public float getClipPadding()
        {
            return this.mClipPadding;
        }

        public void setClipPadding(float clipPadding)
        {
            mClipPadding = clipPadding;
            Invalidate();
        }

        public void setTypeface(Typeface typeface)
        {
            mPaintText.SetTypeface(typeface);
            Invalidate();
        }

        public Typeface getTypeface()
        {
            return mPaintText.Typeface;
        }

        /*
         * (non-Javadoc)
         *
         * @see android.view.View#onDraw(android.graphics.Canvas)
         */
        //@Override
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (mViewPager == null)
            {
                return;
            }
            int count = mViewPager.Adapter.Count;
            if (count == 0)
            {
                return;
            }

            // mCurrentPage is -1 on first start and after orientation changed. If so, retrieve the correct index from viewpager.
            if (mCurrentPage == -1 && mViewPager != null)
            {
                mCurrentPage = mViewPager.CurrentItem;
            }

            //Calculate views bounds
            List<Rect> bounds = calculateAllBounds(mPaintText);
            int boundsSize = bounds.Count;

            //Make sure we're on a page that still exists
            if (mCurrentPage >= boundsSize)
            {
                setCurrentItem(boundsSize - 1);
                return;
            }

            int countMinusOne = count - 1;
            float halfWidth = Width / 2f;
            int left = Left;
            float leftClip = left + mClipPadding;
            int width = Width;
            int height = Height;
            int right = left + width;
            float rightClip = right - mClipPadding;

            int page = mCurrentPage;
            float offsetPercent;
            if (mPageOffset <= 0.5)
            {
                offsetPercent = mPageOffset;
            }
            else
            {
                page += 1;
                offsetPercent = 1 - mPageOffset;
            }
            bool currentSelected = (offsetPercent <= SELECTION_FADE_PERCENTAGE);
            bool currentBold = (offsetPercent <= BOLD_FADE_PERCENTAGE);
            float selectedPercent = (SELECTION_FADE_PERCENTAGE - offsetPercent) / SELECTION_FADE_PERCENTAGE;

            //Verify if the current view must be clipped to the screen
            Rect curPageBound = bounds[mCurrentPage];
            float curPageWidth = curPageBound.Right - curPageBound.Left;
            if (curPageBound.Left < leftClip)
            {
                //Try to clip to the screen (left side)
                clipViewOnTheLeft(curPageBound, curPageWidth, left);
            }
            if (curPageBound.Right > rightClip)
            {
                //Try to clip to the screen (right side)
                clipViewOnTheRight(curPageBound, curPageWidth, right);
            }

            //Left views starting from the current position
            if (mCurrentPage > 0)
            {
                for (int i = mCurrentPage - 1; i >= 0; i--)
                {
                    Rect bound = bounds[i];

                    //Is left side is outside the screen
                    if (bound.Left < leftClip)
                    {
                        int w = bound.Right - bound.Left;
                        //Try to clip to the screen (left side)
                        clipViewOnTheLeft(bound, w, left);
                        //Except if there's an intersection with the right view
                        Rect rightBound = bounds[i + 1];
                        //Intersection
                        if (bound.Right + mTitlePadding > rightBound.Left)
                        {
                            bound.Left = (int)(rightBound.Left - w - mTitlePadding);
                            bound.Right = bound.Left + w;
                        }
                    }
                }
            }
            //Right views starting from the current position
            if (mCurrentPage < countMinusOne)
            {
                for (int i = mCurrentPage + 1; i < count; i++)
                {
                    Rect bound = bounds[i];
                    //If right side is outside the screen
                    if (bound.Right > rightClip)
                    {
                        int w = bound.Right - bound.Left;
                        //Try to clip to the screen (right side)
                        clipViewOnTheRight(bound, w, right);
                        //Except if there's an intersection with the left view
                        Rect leftBound = bounds[i - 1];
                        //Intersection
                        if (bound.Left - mTitlePadding < leftBound.Right)
                        {
                            bound.Left = (int)(leftBound.Right + mTitlePadding);
                            bound.Right = bound.Left + w;
                        }
                    }
                }
            }

            //Now draw views
            //int colorTextAlpha = mColorText >>> 24;
            int colorTextAlpha =  ((int)mColorText) >> 24;
            for (int i = 0; i < count; i++)
            {
                //Get the title
                Rect bound = bounds[i];
                //Only if one side is visible
                if ((bound.Left > left && bound.Left < right) || (bound.Right > left && bound.Right < right))
                {
                    bool currentPage = (i == page);
                    string pageTitle = getTitle(i);

                    //Only set bold if we are within bounds
                    mPaintText.FakeBoldText = currentPage && currentBold && mBoldText;

                    //Draw text as unselected
                    mPaintText.Color = mColorText;
                    if (currentPage && currentSelected)
                    {
                        //Fade out/in unselected text as the selected text fades in/out
                        mPaintText.Alpha = colorTextAlpha - (int)(colorTextAlpha * selectedPercent);
                    }

                    //Except if there's an intersection with the right view
                    if (i < boundsSize - 1)
                    {
                        Rect rightBound = bounds[i + 1];
                        //Intersection
                        if (bound.Right + mTitlePadding > rightBound.Left)
                        {
                            int w = bound.Right - bound.Left;
                            bound.Left = (int)(rightBound.Left - w - mTitlePadding);
                            bound.Right = bound.Left + w;
                        }
                    }
                    canvas.DrawText(pageTitle, 0, pageTitle.Length, bound.Left, bound.Bottom + mTopPadding, mPaintText);

                    //If we are within the selected bounds draw the selected text
                    if (currentPage && currentSelected)
                    {
                        mPaintText.Color = mColorSelected;
                        //mPaintText.Alpha=(int)(mColorSelected >>> 24) * selectedPercent);
                        mPaintText.Alpha = (int)((mColorSelected >> 24) * selectedPercent);
                        canvas.DrawText(pageTitle, 0, pageTitle.Length, bound.Left, bound.Bottom + mTopPadding, mPaintText);
                    }
                }
            }

            //If we want the line on the top change height to zero and invert the line height to trick the drawing code
            float footerLineHeight = mFooterLineHeight;
            float footerIndicatorLineHeight = mFooterIndicatorHeight;
            if (mLinePosition == LinePosition.Top)
            {
                height = 0;
                footerLineHeight = -footerLineHeight;
                footerIndicatorLineHeight = -footerIndicatorLineHeight;
            }

            //Draw the footer line
            mPath.Reset();
            mPath.MoveTo(0, height - footerLineHeight / 2f);
            mPath.LineTo(width, height - footerLineHeight / 2f);
            mPath.Close();
            canvas.DrawPath(mPath, mPaintFooterLine);

            float heightMinusLine = height - footerLineHeight;
            switch (mFooterIndicatorStyle)
            {
                case IndicatorStyle.Triangle:
                    mPath.Reset();
                    mPath.MoveTo(halfWidth, heightMinusLine - footerIndicatorLineHeight);
                    mPath.LineTo(halfWidth + footerIndicatorLineHeight, heightMinusLine);
                    mPath.LineTo(halfWidth - footerIndicatorLineHeight, heightMinusLine);
                    mPath.Close();
                    canvas.DrawPath(mPath, mPaintFooterIndicator);
                    break;

                case IndicatorStyle.Underline:
                    if (!currentSelected || page >= boundsSize)
                    {
                        break;
                    }

                    Rect underlineBounds = bounds[page];
                    float rightPlusPadding = underlineBounds.Right + mFooterIndicatorUnderlinePadding;
                    float leftMinusPadding = underlineBounds.Left - mFooterIndicatorUnderlinePadding;
                    float heightMinusLineMinusIndicator = heightMinusLine - footerIndicatorLineHeight;

                    mPath.Reset();
                    mPath.MoveTo(leftMinusPadding, heightMinusLine);
                    mPath.LineTo(rightPlusPadding, heightMinusLine);
                    mPath.LineTo(rightPlusPadding, heightMinusLineMinusIndicator);
                    mPath.LineTo(leftMinusPadding, heightMinusLineMinusIndicator);
                    mPath.Close();

                    mPaintFooterIndicator.Alpha = (int)(0xFF * selectedPercent);
                    canvas.DrawPath(mPath, mPaintFooterIndicator);
                    mPaintFooterIndicator.Alpha = 0xFF;
                    break;
            }
        }

        public override bool OnTouchEvent(Android.Views.MotionEvent ev)
        {
            if (base.OnTouchEvent(ev))
            {
                return true;
            }
            if ((mViewPager == null) || (mViewPager.Adapter.Count == 0))
            {
                return false;
            }

            MotionEventActions action = ev.Action & MotionEventActions.Mask;
            switch (action)
            {
                case MotionEventActions.Down:
                    mActivePointerId = MotionEventCompat.GetPointerId(ev, 0);
                    mLastMotionX = ev.GetX();
                    break;

                case MotionEventActions.Move:
                    {
                        int activePointerIndex = MotionEventCompat.FindPointerIndex(ev, mActivePointerId);
                        float x = MotionEventCompat.GetX(ev, activePointerIndex);
                        float deltaX = x - mLastMotionX;

                        if (!mIsDragging)
                        {
                            if (Math.Abs(deltaX) > mTouchSlop)
                            {
                                mIsDragging = true;
                            }
                        }

                        if (mIsDragging)
                        {
                            mLastMotionX = x;
                            if (mViewPager.IsFakeDragging || mViewPager.BeginFakeDrag())
                            {
                                mViewPager.FakeDragBy(deltaX);
                            }
                        }

                        break;
                    }

                case MotionEventActions.Cancel:
                case MotionEventActions.Up:
                    if (!mIsDragging)
                    {
                        int count = mViewPager.Adapter.Count;
                        int width = Width;
                        float halfWidth = width / 2f;
                        float sixthWidth = width / 6f;
                        float leftThird = halfWidth - sixthWidth;
                        float rightThird = halfWidth + sixthWidth;
                        float eventX = ev.GetX();

                        if (eventX < leftThird)
                        {
                            if (mCurrentPage > 0)
                            {
                                if (action != MotionEventActions.Cancel)
                                {
                                    mViewPager.SetCurrentItem(mCurrentPage - 1, true);
                                }
                                return true;
                            }
                        }
                        else if (eventX > rightThird)
                        {
                            if (mCurrentPage < count - 1)
                            {
                                if (action != MotionEventActions.Cancel)
                                {
                                    mViewPager.SetCurrentItem(mCurrentPage + 1, true);
                                }
                                return true;
                            }
                        }
                        else
                        {
                            //Middle third
                            if (mCenterItemClickListener != null && action != MotionEventActions.Cancel)
                            {
                                mCenterItemClickListener.onCenterItemClick(mCurrentPage);
                            }
                        }
                    }

                    mIsDragging = false;
                    mActivePointerId = INVALID_POINTER;
                    if (mViewPager.IsFakeDragging) mViewPager.EndFakeDrag();
                    break;

                case MotionEventActions.PointerDown:
                    {
                        int index = MotionEventCompat.GetActionIndex(ev);
                        mLastMotionX = MotionEventCompat.GetX(ev, index);
                        mActivePointerId = MotionEventCompat.GetPointerId(ev, index);
                        break;
                    }

                case MotionEventActions.PointerUp:
                    int pointerIndex = MotionEventCompat.GetActionIndex(ev);
                    int pointerId = MotionEventCompat.GetPointerId(ev, pointerIndex);
                    if (pointerId == mActivePointerId)
                    {
                        int newPointerIndex = pointerIndex == 0 ? 1 : 0;
                        mActivePointerId = MotionEventCompat.GetPointerId(ev, newPointerIndex);
                    }
                    mLastMotionX = MotionEventCompat.GetX(ev, MotionEventCompat.FindPointerIndex(ev, mActivePointerId));
                    break;
            }

            return true;
        }

        /**
         * Set bounds for the right textView including clip padding.
         *
         * @param curViewBound
         *            current bounds.
         * @param curViewWidth
         *            width of the view.
         */
        private void clipViewOnTheRight(Rect curViewBound, float curViewWidth, int right)
        {
            curViewBound.Right = (int)(right - mClipPadding);
            curViewBound.Left = (int)(curViewBound.Right - curViewWidth);
        }

        /**
         * Set bounds for the left textView including clip padding.
         *
         * @param curViewBound
         *            current bounds.
         * @param curViewWidth
         *            width of the view.
         */
        private void clipViewOnTheLeft(Rect curViewBound, float curViewWidth, int left)
        {
            curViewBound.Left = (int)(left + mClipPadding);
            curViewBound.Right = (int)(mClipPadding + curViewWidth);
        }

        /**
         * Calculate views bounds and scroll them according to the current index
         *
         * @param paint
         * @return
         */
        private List<Rect> calculateAllBounds(Paint paint)
        {
            List<Rect> list = new List<Rect>();
            //For each views (If no values then add a fake one)
            int count = mViewPager.Adapter.Count;
            int width = Width;
            int halfWidth = width / 2;
            for (int i = 0; i < count; i++)
            {
                Rect bounds = calcBounds(i, paint);
                int w = bounds.Right - bounds.Left;
                int h = bounds.Bottom - bounds.Top;
                bounds.Left = (int)(halfWidth - (w / 2f) + ((i - mCurrentPage - mPageOffset) * width));
                bounds.Right = bounds.Left + w;
                bounds.Top = 0;
                bounds.Bottom = h;
                list.Add(bounds);
            }

            return list;
        }

        /**
         * Calculate the bounds for a view's title
         *
         * @param index
         * @param paint
         * @return
         */
        private Rect calcBounds(int index, Paint paint)
        {
            //Calculate the text bounds
            Rect bounds = new Rect();
            string title = getTitle(index);
            bounds.Right = (int)paint.MeasureText(title, 0, title.Length);
            bounds.Bottom = (int)(paint.Descent() - paint.Ascent());
            return bounds;
        }

        //@Override
        public void setViewPager(ViewPager view)
        {
            if (mViewPager == view)
            {
                return;
            }
            if (mViewPager != null)
            {
                mViewPager.SetOnPageChangeListener(null);
            }
            if (view.Adapter == null)
            {
                throw new Java.Lang.IllegalStateException("ViewPager does not have adapter instance.");
            }
            mViewPager = view;
            mViewPager.SetOnPageChangeListener(this);
            Invalidate();
        }

        //@Override
        public void setViewPager(ViewPager view, int initialPosition)
        {
            setViewPager(view);
            setCurrentItem(initialPosition);
        }

        //@Override
        public void notifyDataSetChanged()
        {
            Invalidate();
        }

        /**
         * Set a callback listener for the center item click.
         *
         * @param listener Callback instance.
         */
        public void setOnCenterItemClickListener(OnCenterItemClickListener listener)
        {
            mCenterItemClickListener = listener;
        }

        //@Override
        public void setCurrentItem(int item)
        {
            if (mViewPager == null)
            {
                throw new Java.Lang.IllegalStateException("ViewPager has not been bound.");
            }
            mViewPager.SetCurrentItem(item, true);
            mCurrentPage = item;
            Invalidate();
        }

        //@Override
        public void OnPageScrollStateChanged(int state)
        {
            mScrollState = state;

            if (mListener != null)
            {
                mListener.OnPageScrollStateChanged(state);
            }
        }

        //@Override
        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            mCurrentPage = position;
            mPageOffset = positionOffset;
            Invalidate();

            if (mListener != null)
            {
                mListener.OnPageScrolled(position, positionOffset, positionOffsetPixels);
            }
        }

        //@Override
        public void OnPageSelected(int position)
        {
            if (mScrollState == ViewPager.ScrollStateIdle)
            {
                mCurrentPage = position;
                Invalidate();
            }

            if (mListener != null)
            {
                mListener.OnPageSelected(position);
            }
        }

        //@Override
        public void setOnPageChangeListener(ViewPager.IOnPageChangeListener listener)
        {
            mListener = listener;
        }

        //@Override
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            
            //Measure our width in whatever mode specified
            int measuredWidth = MeasureSpec.GetSize(widthMeasureSpec);

            //Determine our height
            float height;
            MeasureSpecMode heightSpecMode = MeasureSpec.GetMode(heightMeasureSpec);
            if (heightSpecMode == MeasureSpecMode.Exactly)
            {
                //We were told how big to be
                height = MeasureSpec.GetSize(heightMeasureSpec);
            }
            else
            {
                //Calculate the text bounds
                mBounds.SetEmpty();
                mBounds.Bottom = (int)(mPaintText.Descent() - mPaintText.Ascent());
                height = mBounds.Bottom - mBounds.Top + mFooterLineHeight + mFooterPadding + mTopPadding;
                if (mFooterIndicatorStyle != IndicatorStyle.None)
                {
                    height += mFooterIndicatorHeight;
                }
            }
            int measuredHeight = (int)height;

            SetMeasuredDimension(measuredWidth, measuredHeight);
        }

        //@Override
        protected override void OnRestoreInstanceState(IParcelable state)
        {
            SavedState savedState = (SavedState)state;
            base.OnRestoreInstanceState(savedState.SuperState);
            mCurrentPage = savedState.currentPage;
            RequestLayout();
        }

        //@Override
        protected override IParcelable OnSaveInstanceState()
        {
            IParcelable superState = base.OnSaveInstanceState();
            SavedState savedState = new SavedState(superState);
            savedState.currentPage = mCurrentPage;
            return savedState;
        }

        class SavedState : BaseSavedState
        {
            public int currentPage;

            public SavedState(IParcelable superState)
                : base(superState)
            {

            }

            private SavedState(Parcel pin)
                : base(pin)
            {

                currentPage = pin.ReadInt();
            }

            //@Override
            public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
            {
                base.WriteToParcel(dest, flags);
                dest.WriteInt(currentPage);
            }

            //@SuppressWarnings("UnusedDeclaration")
            //public static  Parcelable.Creator<SavedState> CREATOR = new Parcelable.Creator<SavedState>() {
            //    @Override
            //    public SavedState createFromParcel(Parcel in) {
            //        return new SavedState(in);
            //    }

            //    @Override
            //    public SavedState[] newArray(int size) {
            //        return new SavedState[size];
            //    }
            //};

            class PCreator : Java.Lang.Object, IParcelableCreator
            {

                public Java.Lang.Object CreateFromParcel(Parcel source)
                {
                    return new SavedState(source);
                }

                public Java.Lang.Object[] NewArray(int size)
                {
                    return new SavedState[size];
                }
            }
            public IParcelableCreator CREATOR = new PCreator();
        }

        private string getTitle(int i)
        {
            string title = mViewPager.Adapter.GetPageTitle(i);
            if (title == null)
            {
                title = EMPTY_TITLE;
            }
            return title;
        }
    }
}