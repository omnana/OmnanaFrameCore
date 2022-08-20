using System.Collections.Generic;
using UnityEngine;

namespace DynamicAtlas
{
    public class SortableSize
    {
        public int width;
        public int height;
        public int id;

        public SortableSize(int width, int height, int id)
        {
            this.width = width;
            this.height = height;
            this.id = id;
        }
    }

    public class IntRect
    {
        public int x;
        public int y;
        public int width;
        public int height;
        public int right;
        public int bottom;
        public int id;

        public IntRect(int x = 0, int y = 0, int width = 0, int height = 0)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.right = x + width;
            this.bottom = y + height;
        }

        public Vector4 GetNormalUvRect(float totalW, float totalH)
        {
            return new Vector4(x / totalW, y / totalH, width / totalW, height / totalH);
        }
        
        public Rect GetRect()
        {
            return new Rect(this.x, this.y, this.width, this.height);
        }
    }

    public class RectPacker
    {
        static public readonly string Version = "1.3.0";

        #region public

        public int rectangleCount
        {
            get { return addRectList.Count; }
        }

        public int packedWidth
        {
            get { return mPackedWidth; }
        }

        public int packedHeight
        {
            get { return mPackedHeight; }
        }

        public int padding
        {
            get { return mPadding; }
        }

        public RectPacker(int width, int height, int padding = 0)
        {
            mOutsideRectangle = new IntRect(width + 1, height + 1, 0, 0);
            Reset(width, height, padding);
        }

        public void Reset(int width, int height, int padding = 0)
        {
            while (addRectList.Count > 0)
                freeRectangle(addRectList.Pop());

            while (mFreeAreas.Count > 0)
                freeRectangle(mFreeAreas.Pop());

            mWidth = width;
            mHeight = height;

            mPackedWidth = 0;
            mPackedHeight = 0;

            mFreeAreas.Add(allocRect(0, 0, mWidth, mHeight));

            while (addSizeList.Count > 0)
                freeSize(addSizeList.Pop());

            mPadding = padding;
        }

        public IntRect CopyRect(int index, IntRect rectForFill)
        {
            IntRect inserted = addRectList[index];

            rectForFill.x = inserted.x;
            rectForFill.y = inserted.y;
            rectForFill.width = inserted.width;
            rectForFill.height = inserted.height;

            return rectForFill;
        }

        public int GetRectId(int index)
        {
            IntRect inserted = addRectList[index];
            return inserted.id;
        }

        public void AddRect(int width, int height, int id)
        {
            SortableSize sortableSize = allocSize(width, height, id);
            addSizeList.Add(sortableSize);
        }

        public int Pack(bool sort = true)
        {
            if (sort)
                addSizeList.Sort((emp1, emp2) => emp1.width.CompareTo(emp2.width));

            while (addSizeList.Count > 0)
            {
                SortableSize sortableSize = addSizeList.Pop();
                int width = sortableSize.width;
                int height = sortableSize.height;

                int index = getFreeAreaIndex(width, height);
                if (index >= 0)
                {
                    IntRect freeArea = mFreeAreas[index];
                    IntRect target = allocRect(freeArea.x, freeArea.y, width, height);
                    target.id = sortableSize.id;

                    generateNewFreeAreas(target, mFreeAreas, mNewFreeAreas);

                    while (mNewFreeAreas.Count > 0)
                        mFreeAreas.Add(mNewFreeAreas.Pop());

                    addRectList.Add(target);

                    if (target.right > mPackedWidth)
                        mPackedWidth = target.right;

                    if (target.bottom > mPackedHeight)
                        mPackedHeight = target.bottom;
                }

                freeSize(sortableSize);
            }

            return rectangleCount;
        }

        #endregion

        #region privatefield

        private int mWidth = 0;
        private int mHeight = 0;
        private int mPadding = 8;

        private int mPackedWidth = 0;
        private int mPackedHeight = 0;

        private List<SortableSize> addSizeList = new List<SortableSize>();

        private List<IntRect> addRectList = new List<IntRect>();
        private List<IntRect> mFreeAreas = new List<IntRect>();
        private List<IntRect> mNewFreeAreas = new List<IntRect>();

        private IntRect mOutsideRectangle;

        private List<SortableSize> mSortableSizeStack = new List<SortableSize>();
        private List<IntRect> rectPool = new List<IntRect>();

        #endregion


        #region privateFunc

        private void filterSelfSubAreas(List<IntRect> areas)
        {
            for (int i = areas.Count - 1; i >= 0; i--)
            {
                IntRect filtered = areas[i];
                for (int j = areas.Count - 1; j >= 0; j--)
                {
                    if (i != j)
                    {
                        IntRect area = areas[j];
                        if (filtered.x >= area.x && filtered.y >= area.y && filtered.right <= area.right && filtered.bottom <= area.bottom)
                        {
                            freeRectangle(filtered);
                            IntRect topOfStack = areas.Pop();
                            if (i < areas.Count)
                            {
                                areas[i] = topOfStack;
                            }

                            break;
                        }
                    }
                }
            }
        }

        private void generateNewFreeAreas(IntRect target, List<IntRect> areas, List<IntRect> results)
        {
            int x = target.x;
            int y = target.y;
            int right = target.right + 1 + mPadding;
            int bottom = target.bottom + 1 + mPadding;

            IntRect targetWithPadding = null;
            if (mPadding == 0)
                targetWithPadding = target;

            for (int i = areas.Count - 1; i >= 0; i--)
            {
                IntRect area = areas[i];
                if (!(x >= area.right || right <= area.x || y >= area.bottom || bottom <= area.y))
                {
                    if (targetWithPadding == null)
                        targetWithPadding = allocRect(target.x, target.y, target.width + mPadding, target.height + mPadding);

                    generateDividedAreas(targetWithPadding, area, results);
                    IntRect topOfStack = areas.Pop();
                    if (i < areas.Count)
                    {
                        areas[i] = topOfStack;
                    }
                }
            }

            if (targetWithPadding != null && targetWithPadding != target)
                freeRectangle(targetWithPadding);

            filterSelfSubAreas(results);
        }

        private void generateDividedAreas(IntRect divider, IntRect area, List<IntRect> results)
        {
            int count = 0;

            int rightDelta = area.right - divider.right;
            if (rightDelta > 0)
            {
                results.Add(allocRect(divider.right, area.y, rightDelta, area.height));
                count++;
            }

            int leftDelta = divider.x - area.x;
            if (leftDelta > 0)
            {
                results.Add(allocRect(area.x, area.y, leftDelta, area.height));
                count++;
            }

            int bottomDelta = area.bottom - divider.bottom;
            if (bottomDelta > 0)
            {
                results.Add(allocRect(area.x, divider.bottom, area.width, bottomDelta));
                count++;
            }

            int topDelta = divider.y - area.y;
            if (topDelta > 0)
            {
                results.Add(allocRect(area.x, area.y, area.width, topDelta));
                count++;
            }

            if (count == 0 && (divider.width < area.width || divider.height < area.height))
            {
                results.Add(area);
            }
            else
                freeRectangle(area);
        }

        private int getFreeAreaIndex(int width, int height)
        {
            IntRect best = mOutsideRectangle;
            int index = -1;

            int paddedWidth = width + mPadding;
            int paddedHeight = height + mPadding;

            int count = mFreeAreas.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                IntRect free = mFreeAreas[i];
                if (free.x < mPackedWidth || free.y < mPackedHeight)
                {
                    if (free.x < best.x && paddedWidth <= free.width && paddedHeight <= free.height)
                    {
                        index = i;
                        if ((paddedWidth == free.width && free.width <= free.height && free.right < mWidth) || (paddedHeight == free.height && free.height <= free.width))
                            break;

                        best = free;
                    }
                }
                else
                {
                    if (free.x < best.x && width <= free.width && height <= free.height)
                    {
                        index = i;
                        if ((width == free.width && free.width <= free.height && free.right < mWidth) || (height == free.height && free.height <= free.width))
                            break;

                        best = free;
                    }
                }
            }

            return index;
        }

        private IntRect allocRect(int x, int y, int width, int height)
        {
            if (rectPool.Count > 0)
            {
                IntRect rectangle = rectPool.Pop();
                rectangle.x = x;
                rectangle.y = y;
                rectangle.width = width;
                rectangle.height = height;
                rectangle.right = x + width;
                rectangle.bottom = y + height;

                return rectangle;
            }

            return new IntRect(x, y, width, height);
        }

        private void freeRectangle(IntRect rectangle)
        {
            rectPool.Add(rectangle);
        }

        private SortableSize allocSize(int width, int height, int id)
        {
            if (mSortableSizeStack.Count > 0)
            {
                SortableSize size = mSortableSizeStack.Pop();
                size.width = width;
                size.height = height;
                size.id = id;

                return size;
            }

            return new SortableSize(width, height, id);
        }

        private void freeSize(SortableSize size)
        {
            mSortableSizeStack.Add(size);
        }

        #endregion
    }

    static class ListExtension
    {
        static public T Pop<T>(this List<T> list)
        {
            int index = list.Count - 1;

            T r = list[index];
            list.RemoveAt(index);
            return r;
        }
    }
}