using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSet : MonoBehaviour
{

    public static Dictionary<BlockType, BlockSettings> Blocks = new Dictionary<BlockType, BlockSettings>(); // dictionary of all blocks available in the game

    public class BlockSettings
    {
        public BlockType type;
        // textures for sides of block
        public float TexUp;
        public float TexDown;
        public float TexForward;
        public float TexBack;
        public float TexLeft;
        public float TexRight;
        // ---------------------------

        // type of block
        public bool isTransparent;

        public bool isFlamming;

        public bool isExplosive;

        public int CustomBlock; // ramps, half-blocks, etc... (0 is default box)

        public Constants.Blocks BlockSound;

        public BlockSettings(float texup, float texdown, float texforward, float texback, float texleft, float texright, bool istransparent, bool isflamming, bool isexplosive, int customblock, BlockType btype, Constants.Blocks blocksound) // init block
        {
            TexUp = texup;
            TexDown = texdown;
            TexForward = texforward;
            TexBack = texback;
            TexLeft = texleft;
            TexRight = texright;
            isTransparent = istransparent;
            isFlamming = isflamming;
            isExplosive = isexplosive;
            CustomBlock = customblock;
            type = btype;
            BlockSound = blocksound;
        }
        public BlockSettings(float tex, bool istransparent, bool isflamming, bool isexplosive, int customblock, BlockType btype, Constants.Blocks blocksound) // init block
        {
            TexUp = tex;
            TexDown = tex;
            TexForward = tex;
            TexBack = tex;
            TexLeft = tex;
            TexRight = tex;
            isTransparent = istransparent;
            isFlamming = isflamming;
            isExplosive = isexplosive;
            CustomBlock = customblock;
            type = btype;
            BlockSound = blocksound;
        }

        public BlockSettings(float tex, BlockType btype, Constants.Blocks blocksound) // init block
        {
            TexUp = tex;
            TexDown = tex;
            TexForward = tex;
            TexBack = tex;
            TexLeft = tex;
            TexRight = tex;
            isTransparent = false;
            isFlamming = false;
            isExplosive = false;
            CustomBlock = 0;
            type = btype;
            BlockSound = blocksound;
        }
        public BlockSettings(float tex, int customblock, BlockType btype, Constants.Blocks blocksound) // init block
        {
            TexUp = tex;
            TexDown = tex;
            TexForward = tex;
            TexBack = tex;
            TexLeft = tex;
            TexRight = tex;
            isTransparent = false;
            isFlamming = false;
            isExplosive = false;
            CustomBlock = customblock;
            type = btype;
            BlockSound = blocksound;
        }
    }

    public void Awake()
    {
        InitBlockSet();
        Debug.Log("Blockset is init", gameObject);
    }

    public void InitBlockSet()
    {
        Blocks.Add(BlockType.Air, new BlockSettings(0, 0, 0, 0, 0, 0, false, false, false, 0, BlockType.Air, Constants.Blocks.Generic));
        Blocks.Add(BlockType.Bedrock, new BlockSettings(1f, 1f, 1f, 1f, 1f, 1f, false, false, false, 0, BlockType.Bedrock, Constants.Blocks.Generic)); // First solid block
        Blocks.Add(BlockType.Stone, new BlockSettings(2f, 2f, 2f, 2f, 2f, 2f, false, false, false, 0, BlockType.Stone, Constants.Blocks.Stone));
        Blocks.Add(BlockType.Dirt, new BlockSettings(3f, 3f, 3f, 3f, 3f, 3f, false, false, false, 0, BlockType.Dirt, Constants.Blocks.Dirt));
        Blocks.Add(BlockType.Sand, new BlockSettings(4, BlockType.Sand, Constants.Blocks.Dirt));
        Blocks.Add(BlockType.Leaves, new BlockSettings(5, false, true, false, 0, BlockType.Leaves, Constants.Blocks.Leaves));
        Blocks.Add(BlockType.Trunk, new BlockSettings(7f, 7f, 6f, 6f, 6f, 6f, false, true, false, 0, BlockType.Trunk, Constants.Blocks.Wood));
        Blocks.Add(BlockType.Wood, new BlockSettings(8, false, true, false, 0, BlockType.Wood, Constants.Blocks.Wood));
        Blocks.Add(BlockType.Grass, new BlockSettings(9f, 3f, 14f, 14f, 14f, 14f, false, false, false, 0, BlockType.Grass, Constants.Blocks.Dirt));
        Blocks.Add(BlockType.TNT, new BlockSettings(11f, 11f, 10f, 10f, 10f, 10f, false, true, true, 0, BlockType.TNT, Constants.Blocks.Wood)); // Explosive
        Blocks.Add(BlockType.DarkStone, new BlockSettings(12, false, false, false, 0, BlockType.DarkStone, Constants.Blocks.Stone));
        Blocks.Add(BlockType.Weeds, new BlockSettings(13f, 3f, 14f, 14f, 14f, 14f, false, false, false, 0, BlockType.Weeds, Constants.Blocks.Dirt));
        Blocks.Add(BlockType.Flowers, new BlockSettings(9f, 3f, 14f, 14f, 14f, 14f, false, false, false, 0, BlockType.Flowers, Constants.Blocks.Dirt)); // old block from version 1.8, no longer used
        Blocks.Add(BlockType.Brick, new BlockSettings(15, BlockType.Brick, Constants.Blocks.Stone));
        Blocks.Add(BlockType.Slate, new BlockSettings(16, BlockType.Slate, Constants.Blocks.Stone));
        Blocks.Add(BlockType.Ice, new BlockSettings(17, BlockType.Ice, Constants.Blocks.Generic)); // Ice
        Blocks.Add(BlockType.Wallpaper, new BlockSettings(18, BlockType.Wallpaper, Constants.Blocks.Generic));
        Blocks.Add(BlockType.Bouncy, new BlockSettings(19, BlockType.Bouncy, Constants.Blocks.Generic));
        Blocks.Add(BlockType.Ladder, new BlockSettings(8f, 8f, 20f, 20f, 20f, 20f, false, true, false, 0, BlockType.Ladder, Constants.Blocks.Wood));
        Blocks.Add(BlockType.Cloud, new BlockSettings(21, BlockType.Cloud, Constants.Blocks.Generic));
        Blocks.Add(BlockType.Water, new BlockSettings(22, true, false, false, 0, BlockType.Water, Constants.Blocks.Water)); // Animated block
        Blocks.Add(BlockType.Fence, new BlockSettings(23, true, false, false, 0, BlockType.Fence, Constants.Blocks.Wood));
        Blocks.Add(BlockType.Ivy, new BlockSettings(24, BlockType.Ivy, Constants.Blocks.Stone));
        Blocks.Add(BlockType.Lava, new BlockSettings(25, BlockType.Lava, Constants.Blocks.Lava)); // Animated block

        Blocks.Add(BlockType.Shingles, new BlockSettings(26, BlockType.Shingles, Constants.Blocks.Generic));
        Blocks.Add(BlockType.NeonSquare, new BlockSettings(27, BlockType.NeonSquare, Constants.Blocks.Generic));
        Blocks.Add(BlockType.Glass, new BlockSettings(28, true, false, false, 0, BlockType.Glass, Constants.Blocks.Glass)); // Transparent block

        Blocks.Add(BlockType.Fireworks, new BlockSettings(8, 8, 29, 29, 29, 29, false, true, true, 0, BlockType.Fireworks, Constants.Blocks.Wood)); // Fireworks, explosive
        Blocks.Add(BlockType.Light, new BlockSettings(30, BlockType.Light, Constants.Blocks.Stone)); // Light/lamp
        Blocks.Add(BlockType.Steel, new BlockSettings(31, BlockType.Steel, Constants.Blocks.Stone));

        // Ramps
        Blocks.Add(BlockType.RockRampEast, new BlockSettings(2, 2, BlockType.RockRampEast, Constants.Blocks.Stone));
        Blocks.Add(BlockType.RockRampNorth, new BlockSettings(2, 2, BlockType.RockRampNorth, Constants.Blocks.Stone));
        Blocks.Add(BlockType.RockRampSouth, new BlockSettings(2, 2, BlockType.RockRampSouth, Constants.Blocks.Stone));
        Blocks.Add(BlockType.RockRampWest, new BlockSettings(2, 2, BlockType.RockRampWest, Constants.Blocks.Stone));
    }
}
