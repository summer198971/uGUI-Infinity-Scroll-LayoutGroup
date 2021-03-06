﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfinityScrollHorizontalLayoutGroup : HorizontalLayoutGroup
{
    public float offset;
    public RectTransform foldRectReference;
    public bool isDrawGizmos;

    Vector3[] mCacheCornerArray;


    public override void SetLayoutHorizontal()
    {
        base.SetLayoutHorizontal();

        for (int i = 0, iMax = rectChildren.Count; i < iMax; i++)
        {
            rectChildren[i].transform.position += Vector3.right * offset;
        }
    }

    void UpdateInfinityScrollOffset()
    {
        if (foldRectReference == null) return;
        if (rectChildren.Count < 2) return;

        var foldWorldRect = GetWorldRectFromFoldRect();
        var expectOffset = 0f;
        var isDirty = false;

        for (int i = rectChildren.Count - 1; i >= 0; i--)
        {
            if (i == rectChildren.Count - 1) continue;

            var item = rectChildren[i];

            if (foldWorldRect.min.x - item.position.x > 0)
            {
                expectOffset += rectChildren[i + 1].position.x - item.position.x;

                item.SetAsLastSibling();
                rectChildren.RemoveAt(i);
                rectChildren.Add(item);
                isDirty = true;
            }
        }

        if(!isDirty)
        {
            for (int i = rectChildren.Count - 1; i >= 0; i--)
            {
                if (i == 0) continue;

                var item = rectChildren[i];

                if (item.position.x - foldWorldRect.max.x > 0)
                {
                    expectOffset += rectChildren[i - 1].position.x - item.position.x;

                    item.SetAsFirstSibling();
                    rectChildren.RemoveAt(i);
                    rectChildren.Insert(0, item);
                }
            }
        }

        offset += expectOffset;
    }

    void Update()
    {
        UpdateInfinityScrollOffset();
    }

    void OnDrawGizmos()
    {
        if (!isDrawGizmos) return;
        if (foldRectReference == null) return;

        var cacheGizmosColor = Gizmos.color;

        Gizmos.color = Color.cyan;

        var worldRect = GetWorldRectFromFoldRect();
        Gizmos.DrawWireCube(worldRect.center, worldRect.size);

        Gizmos.color = cacheGizmosColor;
    }

    Rect GetWorldRectFromFoldRect()
    {
        if (foldRectReference == null) return new Rect();

        if (mCacheCornerArray == null)
            mCacheCornerArray = new Vector3[4];

        // 1 2
        // 0 3
        foldRectReference.GetWorldCorners(mCacheCornerArray);

        return new Rect(
            mCacheCornerArray[1].x
            , mCacheCornerArray[1].y
            , mCacheCornerArray[2].x - mCacheCornerArray[1].x
            , mCacheCornerArray[3].y - mCacheCornerArray[2].y
            );
    }
}
