import { postEvent, type ImpactHapticFeedbackStyle } from "@tma.js/sdk-react";

declare global {
	interface Window {
		coolDown: number | null;
	}
}

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

export function handleCoolDown(action: () => void, coolDown = 300) {
	if (window.coolDown) clearTimeout(window.coolDown);
	window.coolDown = setTimeout(() => {
		window.coolDown = null;
		action();
	}, coolDown);
}

export const isEmptyValue = (value: unknown): boolean => {
	if (value == null) return true;

	if (typeof value === "number") return value === 0;

	if (typeof value === "string") return value.trim() === "";

	if (Array.isArray(value)) {
		return value.every((v) => v === 0);
	}

	return false;
};
