shader_type canvas_item;

instance uniform ivec2 Offset = ivec2(0, 0);
instance uniform bool FacingUp = true;

const vec2 uvScale = vec2(125.f / 2048.f, 175.f / 2048.f);

uniform float SPEED = 1.5f;
uniform float AMPLITUDE = 3.f;

const float VERTEX_OFFSET = 0.2f;

const vec2 BACKING_OFFSET = vec2(0.f, 4.f);
const vec2 BACKFACE_OFFSET = vec2(1.f, 4.f);
const vec2 WHITE_BORDER_OFFSET = vec2(2.f, 4.f);
const vec2 BLACK_BORDER_OFFSET = vec2(3.f, 4.f);

void vertex() {
	float effectiveTime = TIME + (float(VERTEX_ID) * VERTEX_OFFSET);
	VERTEX += vec2(0.f, cos(effectiveTime * SPEED) * AMPLITUDE);
	// Called for every vertex the material is visible on.
}

void fragment() {
	if (FacingUp) {
		vec2 backUv = (UV + BACKING_OFFSET) * uvScale;
		vec2 borderUv = (UV + BLACK_BORDER_OFFSET) * uvScale;
		vec2 transformedUv = (UV + vec2(Offset)) * uvScale;
		vec4 backColor = texture(TEXTURE, backUv);
		vec4 faceColor = texture(TEXTURE, transformedUv);
		vec4 borderColor = texture(TEXTURE, borderUv);
		COLOR = mix(mix(backColor, faceColor, faceColor.w), borderColor, borderColor.w);
	} else {
		vec2 backUv = (UV + BACKFACE_OFFSET) * uvScale;
		COLOR = texture(TEXTURE, backUv);
	}
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
