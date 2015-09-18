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
using Java.Lang;
using System;
using R = ViewPagerIndicator.Net.Resource;




namespace Com.ViewPagerIndicator
{
    /**
     * Draws a line for each page. The current page line is colored differently
     * than the unselected page lines.
     */
    public class UnderlinePageIndicator : View, IPageIndicator
    {
        private static int INVALID_POINTER = -1;
        private static int FADE_FRAME_MS = 30;

        private Paint mPaint = new Paint(PaintFlags.AntiAlias);

        private bool mFades;
        private int mFadeDelay;
        private int mFadeLength;
        private int mFadeBy;

        private ViewPager mViewPager;
        private ViewPager.IOnPageChangeListener mListener;
        private int mScrollState;
        private int mCurrentPage;
        private float mPositionOffset;

        private int mTouchSlop;
        private float mLastMotionX = -1;
        private int mActivePointerId = INVALID_POINTER;
        private bool mIsDragging;

        //private  Runnable mFadeRunnable = new Runnable() {
        //  @Override public void run() {
        //    if (!mFades) return;

        //     int alpha = Math.max(mPaint.getAlpha() - mFadeBy, 0);
        //    mPaint.setAlpha(alpha);
        //    invalidate();
        //    if (alpha > 0) {
        //      postDelayed(this, FADE_FRAME_MS);
        //    }
        //  }
        //};
        private IRunnable mFadeRunnable;
        class FadeRunnable : Java.Lang.Object, IRunnable
        {
            private UnderlinePageIndicator underlinePageIndicator;

            public FadeRunnable(UnderlinePageIndicator underlinePageIndicator)
            {
                // TODO: Complete member initialization
                this.underlinePageIndicator = underlinePageIndicator;
            }

			
            public void Run()
            {
                if (!underlinePageIndicator.mFades) return;

                int alpha = System.Math.Max(underlinePageIndicator.mPaint.Alpha - underlinePageIndicator.mFadeBy, 0);
                underlinePageIndicator.mPaint.Alpha=alpha;
                underlinePageIndicator.Invalidate();
                if (alpha > 0)
                {
                    underlinePageIndicator.PostDelayed(this, FADE_FRAME_MS);
                }
            }
        }

        public UnderlinePageIndicator(Context context)
            : this(context, null)
        {

        }

        public UnderlinePageIndicator(Context context, IAttributeSet attrs)
            : this(context, attrs, R.Attribute.vpiUnderlinePageIndicatorStyle)
        {

        }

        public UnderlinePageIndicator(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {

            mFadeRunnable = new FadeRunnable(this);

            if (IsInEditMode) return;

            Android.Content.Res.Resources res = this.Resources;

            //Load defaults from resources
            bool defaultFades = res.GetBoolean(R.Boolean.default_underline_indicator_fades);
            int defaultFadeDelay = res.GetInteger(R.Integer.default_underline_indicator_fade_delay);
            int defaultFadeLength = res.GetInteger(R.Integer.default_underline_indicator_fade_length);
            int defaultSelectedColor = res.GetColor(R.Color.default_underline_indicator_selected_color);

            //Retrieve styles attributes
            TypedArray a = context.ObtainStyledAttributes(attrs, R.Styleable.UnderlinePageIndicator, defStyle, 0);

            setFades(a.GetBoolean(R.Styleable.UnderlinePageIndicator_fades, defaultFades));
            setSelectedColor(a.GetColor(R.Styleable.UnderlinePageIndicator_selectedColor, defaultSelectedColor));
            setFadeDelay(a.GetInteger(R.Styleable.UnderlinePageIndicator_fadeDelay, defaultFadeDelay));
            setFadeLength(a.GetInteger(R.Styleable.UnderlinePageIndicator_fadeLength, defaultFadeLength));

            Drawable background = a.GetDrawable(R.Styleable.UnderlinePageIndicator_android_background);
            if (background != null)
            {
                SetBackgroundDrawable(background);
            }

            a.Recycle();

            ViewConfiguration configuration = ViewConfiguration.Get(context);
            mTouchSlop = ViewConfigurationCompat.GetScaledPagingTouchSlop(configuration);
        }

        public bool getFades()
        {
            return mFades;
        }

        public void setFades(bool fades)
        {
            if (fades != mFades)
            {
                mFades = fades;
                if (fades)
                {
                    Post(mFadeRunnable);
                }
                else
                {
                    RemoveCallbacks(mFadeRunnable);
                    mPaint.Alpha = 0xFF;
                    Invalidate();
                }
            }
        }

        public int getFadeDelay()
        {
            return mFadeDelay;
        }

        public void setFadeDelay(int fadeDelay)
        {
            mFadeDelay = fadeDelay;
        }

        public int getFadeLength()
        {
            return mFadeLength;
        }

        public void setFadeLength(int fadeLength)
        {
            mFadeLength = fadeLength;
            mFadeBy = 0xFF / (mFadeLength / FADE_FRAME_MS);
        }

        public Color getSelectedColor()
        {
            return mPaint.Color;
        }

        public void setSelectedColor(Color selectedColor)
        {
            mPaint.Color = selectedColor;
            Invalidate();
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

            int paddingLeft = PaddingLeft;
            float pageWidth = (Width - paddingLeft - PaddingRight) / (1f * count);
            float left = paddingLeft + pageWidth * (mCurrentPage + mPositionOffset);
            float right = left + pageWidth;
            float top = PaddingTop;
            float bottom = Height - PaddingBottom;
            canvas.DrawRect(left, top, right, bottom, mPaint);
        }

        public override bool OnTouchEvent(MotionEvent ev)
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
                            if (System.Math.Abs(deltaX) > mTouchSlop)
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



            //throw new NotImplementedException();
            //Post(new Runnable() {
            //    @Override public void run() {
            //        if (mFades) {
            //            post(mFadeRunnable);
            //        }
            //    }
            //});
            Post(new FadeRun(this));
        }

        class FadeRun : Java.Lang.Object, IRunnable
        {
            private UnderlinePageIndicator underlinePageIndicator;

            public FadeRun(UnderlinePageIndicator underlinePageIndicator)
            {
                // TODO: Complete member initialization
                this.underlinePageIndicator = underlinePageIndicator;
            }

             
            public void Run()
            {
                if (underlinePageIndicator.mFades)
                {
                    underlinePageIndicator.Post(underlinePageIndicator.mFadeRunnable);
                }
            }
        }

        //@Override
        public void setViewPager(ViewPager view, int initialPosition)
        {
            setViewPager(view);
            setCurrentItem(initialPosition);
        }

        //@Override
        public void setCurrentItem(int item)
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
        public void notifyDataSetChanged()
        {
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
            mPositionOffset = positionOffset;
            if (mFades)
            {
                if (positionOffsetPixels > 0)
                {
                    RemoveCallbacks(mFadeRunnable);
                    mPaint.Alpha = 0xFF;
                }
                else if (mScrollState != ViewPager.ScrollStateDragging)
                {
                    PostDelayed(mFadeRunnable, mFadeDelay);
                }
            }
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
                mPositionOffset = 0;
                Invalidate();
                mFadeRunnable.Run();
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
            //public static  Creator<SavedState> CREATOR = new Creator<SavedState>() {
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
    }
}