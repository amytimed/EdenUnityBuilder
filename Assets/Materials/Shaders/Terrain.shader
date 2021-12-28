Shader "Eden/Terrain"
{
	Properties
	{
		_MainTex("Texture 1 (white vertices)", 2D) = ""
		_Texture2("Texture 2 (black vertices)", 2D) = ""
	}

		SubShader{
		
		
		Pass{


			Material
			{
				Specular[_SpecColor]
			}
	      
			ColorMaterial AmbientAndDiffuse
			Lighting On
			SeparateSpecular On
			ZWrite On


			SetTexture[_MainTex]
			SetTexture[_Texture2] {Combine previous Lerp(primary) texture}
			SetTexture[nothing] {Combine previous * primary Double}
		}
		
	}
		
}