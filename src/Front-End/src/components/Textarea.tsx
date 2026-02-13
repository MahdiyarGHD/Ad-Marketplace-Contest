import { useRef } from "react";
import { buildClassName } from "../utils/common";

export function Textarea({
	className,
	...props
}: React.TextareaHTMLAttributes<HTMLTextAreaElement>) {
	const ref = useRef<HTMLTextAreaElement>(null);

	if (ref.current) {
		ref.current.style.height = "auto";
		ref.current.style.height = `${ref.current?.scrollHeight}px`;
	}

	return (
		<textarea
			ref={ref}
			className={buildClassName("Textarea", className)}
			{...props}
		/>
	);
}
