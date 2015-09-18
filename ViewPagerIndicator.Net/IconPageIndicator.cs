/*
 * Copyright (C) 2011 The Android Open Source Project
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
//import android.support.v4.view.PagerAdapter;
//import android.support.v4.view.ViewPager;
//import android.support.v4.view.ViewPager.OnPageChangeListener;
//import android.util.AttributeSet;
//import android.view.Gravity;
//import android.view.View;
//import android.widget.HorizontalScrollView;
//import android.widget.ImageView;

using Android.Content;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
//import static android.view.ViewGroup.LayoutParams.FILL_PARENT;
//import static android.view.ViewGroup.LayoutParams.WRAP_CONTENT;
using Android.Widget;
using Java.Lang;
using R=ViewPagerIndicator.Net.Resource;



namespace Com.ViewPagerIndicator
{

    /**
     * This widget implements the dynamic action bar tab behavior that can change
     * across different configurations or circumstances.
     */
    public class IconPageIndicator : HorizontalScrollView, IPageIndicator
    {
        private IcsLinearLayout mIconsLayout;

        private ViewPager mViewPager;
        private Android.Support.V4.View.ViewPager.IOnPageChangeListener mListener;
        private IRunnable mIconSelector;
        private int mSelectedIndex;

        public IconPageIndicator(Context context)
            : this(context, null)
        {

        }

        public IconPageIndicator(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            HorizontalScrollBarEnabled = false;
            //setHorizontalScrollBarEnabled(false);

            mIconsLayout = new IcsLinearLayout(context, R.Attribute.vpiIconPageIndicatorStyle);
            AddView(mIconsLayout, new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent, GravityFlags.Center));
        }

        private void animateToIcon(int position)
        {
            View iconView = mIconsLayout.GetChildAt(position);
            if (mIconSelector != null)
            {
                RemoveCallbacks(mIconSelector);
            }

            //throw new Exception("");
            //mIconSelector = new Runnable() {
            //    public void run() {
            //         int scrollPos = iconView.getLeft() - (getWidth() - iconView.getWidth()) / 2;
            //        smoothScrollTo(scrollPos, 0);
            //        mIconSelector = null;
            //    }
            //};
            //post(mIconSelector);

            mIconSelector = new IconSelector(this,iconView);
            Post(mIconSelector);
        }

        class IconSelector : Java.Lang.Object, IRunnable {
            private IconPageIndicator iconPageIndicator;
            private View iconView;

            public IconSelector(IconPageIndicator iconPageIndicator, View iconView)
            {
                // TODO: Complete member initialization
                this.iconPageIndicator = iconPageIndicator;
                this.iconView = iconView;
            }
                          

            public void Run()
            {
                int scrollPos = iconView.Left - (iconPageIndicator.Width - iconView.Width) / 2;
                iconPageIndicator.SmoothScrollTo(scrollPos, 0);
                iconPageIndicator.mIconSelector = null;
            }
        }

        //@Override
        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            if (mIconSelector != null)
            {
                // Re-post the selector we saved
                Post(mIconSelector);
            }
        }

        //@Override
        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            if (mIconSelector != null)
            {
                RemoveCallbacks(mIconSelector);
            }
        }

        //@Override
        public   void OnPageScrollStateChanged(int arg0)
        {
            if (mListener != null)
            {
                mListener.OnPageScrollStateChanged(arg0);
            }
        }

        //@Override
        public   void OnPageScrolled(int arg0, float arg1, int arg2)
        {
            if (mListener != null)
            {
                mListener.OnPageScrolled(arg0, arg1, arg2);
            }
        }

        //@Override
        public   void OnPageSelected(int arg0)
        {
            setCurrentItem(arg0);
            if (mListener != null)
            {
                mListener.OnPageSelected(arg0);
            }
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
            PagerAdapter adapter = view.Adapter;
            if (adapter == null)
            {
                throw new IllegalStateException("ViewPager does not have adapter instance.");
            }
            mViewPager = view;
            view.SetOnPageChangeListener(this);
            notifyDataSetChanged();
        }

        public void notifyDataSetChanged()
        {
            mIconsLayout.RemoveAllViews();
            IIconPagerAdapter iconAdapter = (IIconPagerAdapter)mViewPager.Adapter;
            int count = iconAdapter.getCount();
            for (int i = 0; i < count; i++)
            {
                ImageView view = new ImageView(this.Context, null, R.Attribute.vpiIconPageIndicatorStyle);
                view.SetImageResource(iconAdapter.getIconResId(i));
                mIconsLayout.AddView(view);
            }
            if (mSelectedIndex > count)
            {
                mSelectedIndex = count - 1;
            }
            setCurrentItem(mSelectedIndex);
            RequestLayout();
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
                throw new IllegalStateException("ViewPager has not been bound.");
            }
            mSelectedIndex = item;
            mViewPager.SetCurrentItem(item, true);

            int tabCount = mIconsLayout.ChildCount;
            for (int i = 0; i < tabCount; i++)
            {
                View child = mIconsLayout.GetChildAt(i);
                bool isSelected = (i == item);
                child.Selected = isSelected;
                //child.setSelected(isSelected);
                if (isSelected)
                {
                    animateToIcon(item);
                }
            }
        }

        //@Override
        public void setOnPageChangeListener(Android.Support.V4.View.ViewPager.IOnPageChangeListener listener)
        {
            mListener = listener;
        }
    }
}