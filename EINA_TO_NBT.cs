﻿using System;
using Substrate;
using Substrate.Nbt;
using Substrate.Core;
using System.IO;
using Substrate.TileEntities;


namespace EINA_TO_NBT
{
    class EINA_TO_NBT
    {
        static int num = 0;
        static void Main (string[] args)
        {

            if (args.Length < 2) {
                Console.WriteLine("Usage: eina_to_nbt <source> <dest>");
                return;
            }

            String dest = args[1];

            System.Console.WriteLine("Creating EINA map...");

            if (!Directory.Exists(dest))
                Directory.CreateDirectory(dest);


            NbtWorld world = AnvilWorld.Create(dest);

            world.Level.LevelName = "EINA";
            world.Level.Spawn = new SpawnPoint(292, 70, 270);
            world.Level.GameType = GameType.CREATIVE;
            world.Level.Initialized = true;


            Player p = new Player();

            p.Position.X = 292;
            p.Position.Y = 130;
            p.Position.Z = 292;

            

            IPlayerManager pm = world.GetPlayerManager();
            pm.SetPlayer("Player",p);

           
            IChunkManager cm = world.GetChunkManager();


            string[] lines = System.IO.File.ReadAllLines(args[0]);

            string[] words; 



            ChunkRef chunk;


            for (int i = 0; i < lines.Length; i++) {

                words = lines[i].Split(' ');
                int x = Int32.Parse(words[0]);
                int y = Int32.Parse(words[1]);
                int z = Int32.Parse(words[2]);
                int color = Int32.Parse(words[3]);
                int xLocal = x/16;
                int yLocal = y/16;
        

                if(!cm.ChunkExists(xLocal,yLocal)){
                    //System.Console.WriteLine(xLocal+"  "+yLocal);
                    cm.CreateChunk(xLocal,yLocal);
                }
                chunk = cm.GetChunkRef(xLocal,yLocal);

                if(!chunk.IsDirty){

                    chunk.IsTerrainPopulated = true;
                    chunk.Blocks.AutoLight = false;
                    //FlatChunk(chunk, 64);
                    chunk.Blocks.RebuildHeightMap();
                    chunk.Blocks.RebuildBlockLight();
                    chunk.Blocks.RebuildSkyLight();
                    //System.Console.WriteLine(chunk.IsDirty);
                    setBlock(chunk,x%16, z + 64, y%16,color);
                }else{
                    //chunk = cm.GetChunkRef(xLocal,yLocal);
                    //System.Console.WriteLine(x%16+"  "+y%16+"  "+(z+64));
                    setBlock(chunk,x%16, z + 64, y%16,color);

                }
            }
            chunk = cm.GetChunkRef(0,0);

            for (int x = 0; x < 16; x++) {
                for (int z = 0; z < 16; z++) {
                    setBlock(chunk,x, 100, z, 16);
                }
            }


            world.Save();


        }



        static void setBlock(ChunkRef chunk, int x, int y, int z, int color){
            if(color<16){
                chunk.Blocks.SetBlock(x, y, z, new AlphaBlock((int)BlockType.WOOL,color));
            }else if(color == 16){
                chunk.Blocks.SetBlock(x, y, z, new AlphaBlock((int)BlockType.GRASS));
            }else if(color == 17){
                chunk.Blocks.SetBlock(x, y, z, new AlphaBlock((int)BlockType.WOOD));
            }else if(color == 18){
                chunk.Blocks.SetBlock(x, y, z, new AlphaBlock((int)BlockType.LEAVES));
            }else if(color == 19){
                AlphaBlock block = new AlphaBlock(63);
                TileEntitySign tile = new TileEntitySign(block.GetTileEntity());
                tile.Text1 = "Ada Byron";
                block.SetTileEntity(tile);
                chunk.Blocks.SetBlock(x, y, z + 1, block);


                block = new AlphaBlock(63,8);
                tile = new TileEntitySign(block.GetTileEntity());
                tile.Text1 = "Ada Byron";
                block.SetTileEntity(tile);
                chunk.Blocks.SetBlock(x, y, z - 1, block);


                block = new AlphaBlock(63,4);
                tile = new TileEntitySign(block.GetTileEntity());
                tile.Text1 = "Ada Byron";
                block.SetTileEntity(tile);
                chunk.Blocks.SetBlock(x - 1, y, z, block);

                block = new AlphaBlock(63,12);
                tile = new TileEntitySign(block.GetTileEntity());
                tile.Text1 = "Ada Byron";
                block.SetTileEntity(tile);
                chunk.Blocks.SetBlock(x + 1, y, z, block);

            }

        }

        static void FlatChunk (ChunkRef chunk, int height)
        {
            // Create bedrock
            for (int y = 0; y < 2; y++) {
                for (int x = 0; x < 16; x++) {
                    for (int z = 0; z < 16; z++) {
                        chunk.Blocks.SetID(x, y, z, (int)BlockType.BEDROCK);
                    }
                }
            }

            // Create stone
            for (int y = 2; y < height - 5; y++) {
                for (int x = 0; x < 16; x++) {
                    for (int z = 0; z < 16; z++) {
                        chunk.Blocks.SetID(x, y, z, (int)BlockType.STONE);
                    }
                }
            }

            // Create dirt
            for (int y = height - 5; y < height - 1; y++) {
                for (int x = 0; x < 16; x++) {
                    for (int z = 0; z < 16; z++) {
                        chunk.Blocks.SetID(x, y, z, (int)BlockType.DIRT);
                    }
                }
            }

            // Create grass
            for (int y = height - 1; y < height; y++) {
                for (int x = 0; x < 16; x++) {
                    for (int z = 0; z < 16; z++) {
                        chunk.Blocks.SetID(x, y, z, (int)BlockType.GRASS);
                    }
                }
            }
        }
    }
}