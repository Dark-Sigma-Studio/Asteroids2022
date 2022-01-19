#version 430
//-------------- Defines --------------//
#define iCoord gl_FragCoord.xy
//-------------------------------------//

const float PI = atan(0.0, -1.0);

//------------- Uniforms -------------//
uniform vec2 iResolution = vec2(800.0, 600.0);

uniform vec2 tplaypos = vec2(0.0);
uniform float tplayangle = 0.0;
uniform vec2 campos = vec2(0.0);

uniform vec2[32] bullets;
uniform uint bullnumb;
//------------------------------------//
out vec4 fragcol;

float Player(in vec2 uv)
{
	float d = 0.0;

	vec2 trans = tplaypos - uv;

	float rad = length(trans);
	float a = 1.0 - abs(fract(((atan(trans.y, trans.x) - tplayangle) / PI + 1.0) / 2.0) * 2.0 - 1.0);

	d = a - (rad) + 0.5;

	float ring = 1.0 - abs(1.0 - rad * 10.0) * 5.0;
	float point = 1.0 - a / rad;

	d = clamp(ring + point, 0.0, 1.0)+ ring;

	return d;
}

float border(in vec2 uv)
{
	float d = 0.0;

	d = max(abs(fract(uv.x) - 0.5) * 2.0, abs(fract(uv.y) - 0.5) * 2.0);

	float width = 50.0;

	d = (((d - 1.0) * width) + 1.0);

	return d;
}

float bullet(in vec2 uv)
{
	float d = 0.0;

	for(int i = 0; i < bullnumb; i++)
	{
		float r = distance(uv, bullets[i]);
		r *= 50.0;
		r = 1.0 - r;

		d = max(d, r);
	}

	return d;
}

void main()
{
	vec2 uv = 2.0 * (iCoord - 0.5 * iResolution) / 800.0 + campos;

	float col = 0.0;

	col = clamp(Player(uv), 0.0, 1.0);
	col += clamp(border(uv), 0.0, 1.0);
	col += clamp(bullet(uv), 0.0, 1.0);

	fragcol = vec4(vec3(col), 1.0);
}