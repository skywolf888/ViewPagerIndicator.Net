/*
 * Copyright (C) 2012 Jake Wharton
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
//import android.graphics.drawable.Drawable;
//import android.os.Parcel;
//import android.os.Parcelable;
//import android.support.v4.view.MotionEventCompat;
//import android.support.v4.view.ViewConfigurationCompat;
//import android.support.v4.view.ViewPager;
//import android.util.AttributeSet;
//import android.util.FloatMath;
//import android.view.MotionEvent;
//import android.view.View;
//import android.view.ViewConfiguration;



using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using System;
using R=ViewPagerIndicator.Net.Resource;

namespace Com.ViewPagerIndicator
{
    /**
     * Draws a line for each page. The current page line is colored differently
     * than the unselected page lines.
     */
    public class LinePageIndicator : View, IPageIndicator
    {
        private static int INVALID_POINTER = -1;

        private Paint mPaintUnselected = new Paint(PaintFlags.AntiAlias);
        private Paint mPaintSelected = new Paint(PaintFlags.AntiAlias);
        private ViewPager mViewPager;
        private ViewPager.IOnPageChangeListener mListener;
        private int mCurrentPage;
        private bool mCentered;
        private float mLineWidth;
        private float mGapWidth;

        private int mTouchSlop;
        private float mLastMotionX = -1;
        private int mActivePointerId = INVALID_POINTER;
        private bool mIsDragging;


        public LinePageIndicator(Context context)
            : this(context, null)
        {

        }

        public LinePageIndicator(Context context, IAttributeSet attrs)
            : this(context, attrs, R.Attribute.vpiLinePageIndicatorStyle)
        {

        }

        public LinePageIndicator(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {

            if (IsInEditMode) return;

            Android.Content.Res.Resources res = this.Resources;

            //Load defaults from resources
            int defaultSelectedColor = res.GetColor(R.Color.default_line_indicator_selected_color);
            int defaultUnselectedColor = res.GetColor(R.Color.default_line_indicator_unselected_color);
            float defaultLineWidth = res.GetDimension(R.Dimension.default_line_indicator_line_width);
            float defaultGapWidth = res.GetDimension(R.Dimension.default_line_indicator_gap_width);
            float defaultStrokeWidth = res.GetDimension(R.Dimension.default_line_indicator_stroke_width);
            bool defaultCentered = res.GetBoolean(R.Boolean.default_line_indicator_centered);

            //Retrieve styles attributes
            TypedArray a = context.ObtainStyledAttributes(attrs, R.Styleable.LinePageIndicator, defStyle, 0);

            mCentered = a.GetBoolean(R.Styleable.LinePageIndicator_centered, defaultCentered);
            mLineWidth = a.GetDimension(R.Styleable.LinePageIndicator_lineWidth, defaultLineWidth);
            mGapWidth = a.GetDimension(R.Styleable.LinePageIndicator_gapWidth, defaultGapWidth);
            setStrokeWidth(a.GetDimension(R.Styleable.LinePageIndicator_strokeWidth, defaultStrokeWidth));
            mPaintUnselected.Color = a.GetColor(R.Styleable.LinePageIndicator_unselectedColor, defaultUnselectedColor);
            mPaintSelected.Color = a.GetColor(R.Styleable.LinePageIndicator_selectedColor, defaultSelectedColor);

            Drawable background = a.GetDrawable(R.Styleable.LinePageIndicator_android_background);
            if (background != null)
            {
                SetBackgroundDrawable(background);
            }

            a.Recycle();

            ViewConfiguration configuration = ViewConfiguration.Get(context);
            mTouchSlop = ViewConfigurationCompat.GetScaledPagingTouchSlop(configuration);
        }


        public void setCentered(bool centered)
        {
            mCentered = centered;
            Invalidate();
        }

        public bool isCentered()
        {
            return mCentered;
        }

        public void setUnselectedColor(Color unselectedColor)
        {
            mPaintUnselected.Color = unselectedColor;
            Invalidate();
        }

        public Color getUnselectedColor()
        {
            return mPaintUnselected.Color;
        }

        public void setSelectedColor(Color selectedColor)
        {
            mPaintSelected.Color = selectedColor;
            Invalidate();
        }

        public Color getSelectedColor()
        {
            return mPaintSelected.Color;
        }

        public void setLineWidth(float lineWidth)
        {
            mLineWidth = lineWidth;
            Invalidate();
        }

        public float getLineWidth()
        {
            return mLineWidth;
        }

        public void setStrokeWidth(float lineHeight)
        {
            mPaintSelected.StrokeWidth = lineHeight;
            mPaintUnselected.StrokeWidth = lineHeight;
            Invalidate();
        }

        public float getStrokeWidth()
        {
            return mPaintSelected.StrokeWidth;
        }

        public void setGapWidth(float gapWidth)
        {
            mGapWidth = gapWidth;
            Invalidate();
        }

        public float getGapWidth()
        {
            return mGapWidth;
        }

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

            if (mCurrentPage >= count)
            {
                setCurrentItem(count - 1);
                return;
            }

            float lineWidthAndGap = mLineWidth + mGapWidth;
            float indicatorWidth = (count * lineWidthAndGap) - mGapWidth;
            float paddingTop = PaddingTop;
            float paddingLeft = PaddingLeft;
            float paddingRight = PaddingRight;

            float verticalOffset = paddingTop + ((Height - paddingTop - PaddingBottom) / 2.0f);
            float horizontalOffset = paddingLeft;
            if (mCentered)
            {
                horizontalOffset += ((Width - paddingLeft - paddingRight) / 2.0f) - (indicatorWidth / 2.0f);
            }

            //Draw stroked circles
            for (int i = 0; i < count; i++)
            {
                float dx1 = horizontalOffset + (i * lineWidthAndGap);
                float dx2 = dx1 + mLineWidth;
                canvas.DrawLine(dx1, verticalOffset, dx2, verticalOffset, (i == mCurrentPage) ? mPaintSelected : mPaintUnselected);
            }
        }

        public bool onTouchEvent(Android.Views.MotionEvent ev)
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

                        if ((mCurrentPage > 0) && (ev.GetX() < halfWidth - sixthWidth))
                        {
                            if (action != MotionEventActions.Cancel)
                            {
                                mViewPager.SetCurrentItem(mCurrentPage - 1,true);
                            }
                            return true;
                        }
                        else if ((mCurrentPage < count - 1) && (ev.GetX() > halfWidth + sixthWidth))
                        {
                            if (action != MotionEventActions.Cancel)
                            {
                                mViewPager.SetCurrentItem(mCurrentPage + 1,true);
                            }
                            return true;
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

        //@Override
        public   void setViewPager(ViewPager viewPager)
        {
            if (mViewPager == viewPager)
            {
                return;
            }
            if (mViewPager != null)
            {
                //Clear us from the old pager.
                mViewPager.SetOnPageChangeListener(null);
            }
            if (viewPager.Adapter == null)
            {
                throw new Java.Lang.IllegalStateException("ViewPager does not have adapter instance.");
            }
            mViewPager = viewPager;
            mViewPager.SetOnPageChangeListener(this);
            Invalidate();
        }

        //@Override
        public   void setViewPager(ViewPager view, int initialPosition)
        {
            setViewPager(view);
            setCurrentItem(initialPosition);
        }

        //@Override
        public   void setCurrentItem(int item)
        {
            if (mViewPager == null)
            {
                throw new Java.Lang.IllegalStateException("ViewPager has not been bound.");
            }
            mViewPager.SetCurrentItem(item,true);
            mCurrentPage = item;
            Invalidate();
        }

        //@Override
        public   void notifyDataSetChanged()
        {
            Invalidate();
        }

        //@Override
        public void OnPageScrollStateChanged(int state)
        {
            if (mListener != null)
            {
                mListener.OnPageScrollStateChanged(state);
            }
        }

        //@Override
        public   void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            if (mListener != null)
            {
                mListener.OnPageScrolled(position, positionOffset, positionOffsetPixels);
            }
        }

        //@Override
        public void OnPageSelected(int position)
        {
            mCurrentPage = position;
            Invalidate();

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
            SetMeasuredDimension(measureWidth(widthMeasureSpec), measureHeight(heightMeasureSpec));
        }

        /**
         * Determines the width of this view
         *
         * @param measureSpec
         *            A measureSpec packed into an int
         * @return The width of the view, honoring constraints from measureSpec
         */
        private int measureWidth(int measureSpec)
        {
            float result;
            MeasureSpecMode specMode = MeasureSpec.GetMode(measureSpec);
            int specSize = MeasureSpec.GetSize(measureSpec);

            if ((specMode == MeasureSpecMode.Exactly) || (mViewPager == null))
            {
                //We were told how big to be
                result = specSize;
            }
            else
            {
                //Calculate the width according the views count
                int count = mViewPager.Adapter.Count;
                result = PaddingLeft + PaddingRight + (count * mLineWidth) + ((count - 1) * mGapWidth);
                //Respect AT_MOST value if that was what is called for by measureSpec
                if (specMode == MeasureSpecMode.AtMost)
                {
                    result = Math.Min(result, specSize);
                }
            }
            return (int)FloatMath.Ceil(result);
        }

        /**
         * Determines the height of this view
         *
         * @param measureSpec
         *            A measureSpec packed into an int
         * @return The height of the view, honoring constraints from measureSpec
         */
        private int measureHeight(int measureSpec)
        {
            float result;
            MeasureSpecMode specMode = MeasureSpec.GetMode(measureSpec);
            int specSize = MeasureSpec.GetSize(measureSpec);

            if (specMode == MeasureSpecMode.Exactly)
            {
                //We were told how big to be
                result = specSize;
            }
            else
            {
                //Measure the height
                result = mPaintSelected.StrokeWidth + PaddingTop + PaddingBottom;
                //Respect AT_MOST value if that was what is called for by measureSpec
                if (specMode == MeasureSpecMode.AtMost)
                {
                    result = Math.Min(result, specSize);
                }
            }
            return (int)FloatMath.Ceil(result);
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
        }
    }
}