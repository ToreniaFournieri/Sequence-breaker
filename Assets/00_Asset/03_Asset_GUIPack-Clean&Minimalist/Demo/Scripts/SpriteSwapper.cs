// Copyright (C) 2015-2017 ricimi - All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement.
// A Copy of the Asset Store EULA is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using UnityEngine.UI;

namespace _00_Asset.Demo.Scripts
{
    // Utility class for swapping the sprite of a UI Image between two predefined values.
    public class SpriteSwapper : MonoBehaviour
    {
        public Sprite enabledSprite;
        public Sprite disabledSprite;

        private bool _mSwapped = true;

        private Image _mImage;

        public void Awake()
        {
            _mImage = GetComponent<Image>();
        }

        public void SwapSprite()
        {
            if (_mSwapped)
            {
                _mSwapped = false;
                _mImage.sprite = disabledSprite;
            }
            else
            {
                _mSwapped = true;
                _mImage.sprite = enabledSprite;
            }
        }
    }
}
