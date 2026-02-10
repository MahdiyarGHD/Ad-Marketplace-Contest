import { buildClassName } from "../utils/common";

export function Shimmer({
	state = true,
	className,
	children,
}: {
	state?: boolean;
	className?: string;
	children?: React.ReactNode;
}) {
	return (
		<div
			className={buildClassName((!children || !state) && "Shimmer", className)}
		>
			{state && children}
		</div>
	);
}
