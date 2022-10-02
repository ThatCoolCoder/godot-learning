using System;
using Godot;

namespace Physics.Fluids
{
	public class Sea : MeshInstance, ISpatialFluid
	{
		// Pretty ordinary sea. 
		// Currently has no waves, we need to port the code from the shader over

		[Export] public float BaseDensity { get; set; } = 1.0f;
		[Export] public Vector3 Flow { get; set; } = Vector3.Zero;

		private ShaderMaterial material;
		private float scale;
		private float heightScale;
		private Image noise;
		private float time = 0;
		private Image waveMap1;
		private Image waveMap2;
		private Image waveMap3;
		private Vector2 waveAngle1;
		private Vector2 waveAngle2;
		private Vector2 waveAngle3;
		private float waveSpeed;
		private float waveHeightScale;

		public override void _Ready()
		{

			base._Ready();
		}

		private void InitStuff()
		{
			material = GetSurfaceMaterial(0) as ShaderMaterial;
			scale = (float) material.GetShaderParam("scale");
			heightScale = (float) material.GetShaderParam("height_scale");
			noise = (material.GetShaderParam("noise") as Texture).GetData();
			// (we do not need to fetch time from the shader)
			waveMap1 = (material.GetShaderParam("wave_height_1") as NoiseTexture).GetData();
			waveMap2 = (material.GetShaderParam("wave_height_2") as Texture).GetData();
			waveMap3 = (material.GetShaderParam("wave_height_3") as Texture).GetData();
			waveAngle1 = (Vector2) material.GetShaderParam("wave_angle_1");
			waveAngle2 = (Vector2) material.GetShaderParam("wave_angle_2");
			waveAngle3 = (Vector2) material.GetShaderParam("wave_angle_3");
			waveSpeed = (float) material.GetShaderParam("wave_speed");
			waveHeightScale = (float) material.GetShaderParam("wave_height_scale");
		}

		public override void _PhysicsProcess(float delta)
		{
			time += delta;
			if (material != null) material.SetShaderParam("global_time", time);

			base._Process(delta);
		}

		public float HeightAtPoint(Vector3 point)
		{
			if (time < 0.2f)
			{
				InitStuff();
				return GlobalTransform.origin.y;
			}

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
				pos /= scale * 2;
				pos.x += 0.5f;
				pos.y += 0.5f;
				return pos;
			}

			float WaveHeightOffset(Vector2 pos, Image heightMap, Vector2 waveDirection, float time)
			{
				var movement = waveDirection.Normalized() * time * waveSpeed;
				pos += movement;
				var normalizedPos = pos / heightMap.GetSize().x;

				var height = ReadPixelValue(heightMap, normalizedPos);
				height *= height;
				height *= waveHeightScale / heightScale;
				return height;
			}

			float HeightAtPos(Vector2 pos, float time)
			{
				var normalizedPos = TexturePosFromWorld(pos);
				float height = ReadPixelValue(noise, normalizedPos);
				height += WaveHeightOffset(pos, waveMap1, waveAngle1, time);
				height += WaveHeightOffset(pos, waveMap2, waveAngle2, time);
				height += WaveHeightOffset(pos, waveMap3, waveAngle3, time);

				return (height - 0.5f) * heightScale;
			}
			
			var globalPos = GlobalTransform.origin;
			return globalPos.y + HeightAtPos(new Vector2(point.x, point.z), time);
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
