Shader "Eden/VertexTerrainTransperent"
{
	Properties
	{
		_SpecColor("Specular Color", Color) = (1,1,1,1)
		_Shininess("Shininess", Range(0,1)) = 0.7
		_Emission("Emmisive Color", Color) = (0,0,0)
		_MainTex("Texture 1 (white vertices)", 2D) = ""
		_Texture2("Texture 2 (black vertices)", 2D) = ""
	}

		SubShader{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}

		Pass{


			Material
			{
				Specular[_SpecColor]
				Shininess[_Shininess]
				Emission[_Emission]
			}

			ColorMaterial AmbientAndDiffuse
			Lighting On
			SeparateSpecular On
			ZWrite On
			Blend SrcAlpha OneMinusSrcAlpha

			SetTexture[_MainTex]
			SetTexture[_Texture2] {Combine previous Lerp(primary) texture}
			SetTexture[nothing] {Combine previous * primary Double}
		}

	}

}