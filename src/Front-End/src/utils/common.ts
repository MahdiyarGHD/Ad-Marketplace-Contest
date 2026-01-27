import { postEvent, type ImpactHapticFeedbackStyle } from "@tma.js/sdk-react";

export function buildClassName(
	...params: (string | false | undefined)[]
): string {
	return params.filter(Boolean).join(" ");
}

export function numberWithCommas(x?: number) {
	return x?.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

export function invokeHapticFeedbackImpact(style: ImpactHapticFeedbackStyle) {
	postEvent("web_app_trigger_haptic_feedback", {
		type: "impact",
		impact_style: style,
	});
}
