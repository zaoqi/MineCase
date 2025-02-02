using System;
using MineCase.Algorithm.World.Plants;
using MineCase.Block;
using MineCase.Server.Game.Entities;
using MineCase.Server.World;
using MineCase.Server.World.EntitySpawner;
using MineCase.World;
using MineCase.World.Biomes;
using MineCase.World.Generation;
using MineCase.World.Plants;
using Orleans;

namespace MineCase.Algorithm.World.Biomes
{
    public class BiomeSwamp : Biome
    {
        public BiomeSwamp(BiomeProperties properties, GeneratorSettings genSettings)
            : base(properties, genSettings)
        {
            _name = "swampland";
            _biomeId = BiomeId.Swampland;

            _treesPerChunk = 2;
            _flowersPerChunk = 1;
            _deadBushPerChunk = 1;
            _mushroomsPerChunk = 8;
            _reedsPerChunk = 10;
            _clayPerChunk = 1;
            _waterlilyPerChunk = 4;
            _sandPatchesPerChunk = 0;
            _gravelPatchesPerChunk = 0;
            _grassPerChunk = 5;

            _baseHeight = -0.2F;
            _heightVariation = 0.1F;
            _temperature = 0.8F;
            _rainfall = 0.9F;
            _enableRain = true;
            _waterColor = 14745518;
        }

        // 添加其他东西
        public override void Decorate(IWorld world, IGrainFactory grainFactory, ChunkColumnCompactStorage chunk, Random rand, BlockWorldPos pos)
        {
            float grassColor = (_grassColorNoise.Noise((pos.X + 8) / 200.0F, 0.0F, (pos.Z + 8) / 200.0F) - 0.5F) * 2;

            if (grassColor < -0.8F)
            {
                _flowersPerChunk = 15;
                _grassPerChunk = 5 * 7;
                GenDoubleFlowers(world, grainFactory, chunk, rand, pos);
            }
            else
            {
                _flowersPerChunk = 4;
                _grassPerChunk = 10 * 7;
            }

            GenGrass(world, grainFactory, chunk, rand, pos);
            GenFlowers(world, grainFactory, chunk, rand, pos);
            GenDoubleGrass(world, grainFactory, chunk, rand, pos);

            int treesPerChunk = _treesPerChunk;

            if (rand.NextDouble() < _extraTreeChance)
            {
                ++treesPerChunk;
            }

            for (int num = 0; num < treesPerChunk; ++num)
            {
                int x = rand.Next(10) + 3;
                int z = rand.Next(10) + 3;

                TreeGenerator treeGenerator = new TreeGenerator(5, true, GetRandomTree(rand));

                // 获得地表面高度
                int h = 0;
                for (int y = 255; y >= 0; --y)
                {
                    if (!chunk[x, y, z].IsAir())
                    {
                        h = y + 1;
                        break;
                    }
                }

                treeGenerator.Generate(world, grainFactory, chunk, this, rand, new BlockWorldPos(pos.X + x, h, pos.Z + z));
            }

            base.Decorate(world, grainFactory, chunk, rand, pos);
        }

        // 添加生物群系特有的生物
        public override void SpawnMob(IWorld world, IGrainFactory grainFactory, ChunkColumnCompactStorage chunk, Random rand, BlockWorldPos pos)
        {
            ChunkWorldPos chunkPos = pos.ToChunkWorldPos();
            int seed = chunkPos.Z * 16384 + chunkPos.X;
            Random r = new Random(seed);
            foreach (MobType eachType in _passiveMobList)
            {
                if (r.Next(32) == 0)
                {
                    PassiveMobSpawner spawner = new PassiveMobSpawner(eachType, 10);
                    spawner.Spawn(world, grainFactory, chunk, rand, new BlockWorldPos(pos.X, pos.Y, pos.Z));
                }
            }
        }

        private void GenGrass(IWorld world, IGrainFactory grainFactory, ChunkColumnCompactStorage chunk, Random random, BlockWorldPos pos)
        {
            int grassMaxNum = random.Next(_grassPerChunk);
            GrassGenerator generator = new GrassGenerator();
            for (int grassNum = 0; grassNum < grassMaxNum; ++grassNum)
            {
                int x = random.Next(16);
                int z = random.Next(16);
                for (int y = 255; y >= 1; --y)
                {
                    if (!chunk[x, y, z].IsAir())
                    {
                        generator.Generate(world, grainFactory, chunk, this, random, new BlockWorldPos(pos.X + x, y + 1, pos.Z + z));
                        break;
                    }
                }
            }
        }

        private void GenFlowers(IWorld world, IGrainFactory grainFactory, ChunkColumnCompactStorage chunk, Random random, BlockWorldPos pos)
        {
            int flowersMaxNum = random.Next(_flowersPerChunk);
            FlowersGenerator generator = new FlowersGenerator();
            for (int flowersNum = 0; flowersNum < flowersMaxNum; ++flowersNum)
            {
                int x = random.Next(16);
                int z = random.Next(16);
                for (int y = 255; y >= 1; --y)
                {
                    if (!chunk[x, y, z].IsAir())
                    {
                        generator.Generate(world, grainFactory, chunk, this, random, new BlockWorldPos(pos.X + x, y + 1, pos.Z + z));
                        break;
                    }
                }
            }
        }

        private void GenDoubleFlowers(IWorld world, IGrainFactory grainFactory, ChunkColumnCompactStorage chunk, Random random, BlockWorldPos pos)
        {
            DoubleFlowersGenerator generator = new DoubleFlowersGenerator(PlantsType.Sunflower);
            for (int flowersNum = 0; flowersNum < 10; ++flowersNum)
            {
                int x = random.Next(16);
                int z = random.Next(16);
                for (int y = 255; y >= 1; --y)
                {
                    if (!chunk[x, y, z].IsAir())
                    {
                        generator.Generate(world, grainFactory, chunk, this, random, new BlockWorldPos(pos.X + x, y + 1, pos.Z + z));
                        break;
                    }
                }
            }
        }

        private void GenDoubleGrass(IWorld world, IGrainFactory grainFactory, ChunkColumnCompactStorage chunk, Random random, BlockWorldPos pos)
        {
            DoubleGrassGenerator generator = new DoubleGrassGenerator(PlantsType.DoubleTallgrass);
            for (int grassNum = 0; grassNum < 2; ++grassNum)
            {
                int x = random.Next(16);
                int z = random.Next(16);
                for (int y = 255; y >= 1; --y)
                {
                    if (!chunk[x, y, z].IsAir())
                    {
                        generator.Generate(world, grainFactory, chunk, this, random, new BlockWorldPos(pos.X + x, y + 1, pos.Z + z));
                        break;
                    }
                }
            }
        }
    }
}