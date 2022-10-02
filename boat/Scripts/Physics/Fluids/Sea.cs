using System;
using Godot;

namespace Physics.Fluids
{
	public class Sea : MeshInstance, ISpatialFluid
	{
		// Pretty ordinary sea.
		// We have copies of the Textures as Images so we can import them from GPU to CPU.

		// IMPORTANT: when modifying the math, please modify water.gdshader to reflect the changes,
		// or physics and graphics will desync!

		[Export] public float BaseDensity { get; set; } = 1.0f;
		[Export] public Vector3 Flow { get; set; } = Vector3.Zero;

		private ShaderMaterial material;

		[Export] public float VertexScale;
		[Export] public float HeightScale;
		[Export] public Texture Noise;
		[Export] public int NoiseSize;
		private Image noiseImage;
		[Export] public float Time = 0;

		[Export] public int WaveMapSize;
		[Export] public Texture WaveMap1;
		private Image waveMap1Image;
		[Export] public Texture WaveMap2;
		private Image waveMap2Image;
		[Export] public Texture WaveMap3;
		private Image waveMap3Image;

		[Export] public Vector2 WaveAngle1 = new Vector2(0, 1);
		[Export] public Vector2 WaveAngle2 = new Vector2(0.5f, 0.866f);
		[Export] public Vector2 WaveAngle3 = new Vector2(-0.5f, 0.866f);

		[Export] public float WaveSpeed;
		[Export] public float WaveHeightScale;

		public override void _Ready()
		{
			UpdateShaderParams();
			base._Ready();
		}

		private void UpdateShaderParams()
		{
			material = GetSurfaceMaterial(0) as ShaderMaterial;

			material.SetShaderParam("scale", VertexScale);
			material.SetShaderParam("height_scale", HeightScale);
			material.SetShaderParam("noise", Noise);

			material.SetShaderParam("wave_map_size", WaveMapSize);
			material.SetShaderParam("wave_height_1", WaveMap1);
			material.SetShaderParam("wave_height_2", WaveMap2);
			material.SetShaderParam("wave_height_3", WaveMap3);

			material.SetShaderParam("wave_angle_1", WaveAngle1);
			material.SetShaderParam("wave_angle_2", WaveAngle2);
			material.SetShaderParam("wave_angle_3", WaveAngle3);

			material.SetShaderParam("wave_speed", WaveSpeed);
			material.SetShaderParam("wave_height_scale", WaveHeightScale);
		}

		private void TryGetNoiseImagesFromGpu()
		{
			// Attempt to get the noise images from the gpu.
			noiseImage = Noise.GetData();
			waveMap1Image = WaveMap1.GetData();
			waveMap2Image = WaveMap2.GetData();
			waveMap3Image = WaveMap3.GetData();
		}

		public override void _PhysicsProcess(float delta)
		{
			Time += delta;
			TryGetNoiseImagesFromGpu();
			if (material != null) material.SetShaderParam("global_time", Time);

			base._Process(delta);
		}

		public float HeightAtPoint(Vector3 point)
		{
			if (noiseImage == null || waveMap1Image == null || waveMap2Image == null || waveMap3Image == null) return GlobalTransform.origin.y;

			float ReadPixelValue(Image image, Vector2 uv)
			{
				var x = Mathf.PosMod(uv.x, 1) * image.GetWidth();
				var y = Mathf.PosMod(uv.y, 1) * image.GetHeight();
				image.Lock();
				var pixel = image.GetPixel((int) x, (int) y);
				image.Unlock();
				return pixel.r;
			}

			Vector2 TexturePosFromWorld(Vector2 pos)
			{
				pos /= VertexScale * 2;
				pos.x += 0.5f;
				pos.y += 0.5f;
				return pos;
			}

			float WaveHeightOffset(Vector2 pos, Image heightMap, Vector2 waveDirection, float time)
			{
				var movement = waveDirection.Normalized() * time * WaveSpeed;
				pos += movement;
				var normalizedPos = pos / heightMap.GetSize().x;

				var height = ReadPixelValue(heightMap, normalizedPos);
				height *= height;
				height *= WaveHeightScale / HeightScale;
				return height;
			}

			float HeightAtPos(Vector2 pos, float time)
			{
				var normalizedPos = TexturePosFromWorld(pos);
				float height = ReadPixelValue(noiseImage, normalizedPos);
				height += WaveHeightOffset(pos, waveMap1Image, WaveAngle1, time);
				height += WaveHeightOffset(pos, waveMap2Image, WaveAngle2, time);
				height += WaveHeightOffset(pos, waveMap3Image, WaveAngle3, time);

				return (height - 0.5f) * HeightScale;
			}
			
			var globalPos = GlobalTransform.origin;
			return globalPos.y + HeightAtPos(new Vector2(point.x, point.z), Time);
		}

		public float DensityAtPoint(Vector3 point)
		{
			return BaseDensity;
		}

		public Vector3 VelocityAtPoint(Vector3 point)
		{
			return Flow;
		}

		public FluidType Type { get; set; } = FluidType.Gas;
	}
}
