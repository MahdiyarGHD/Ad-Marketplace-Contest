import { memo, useState } from "react";
import { buildClassName } from "../utils/common";
import { LoaderCircle } from "lucide-react";

function MainButton({
	text,
	onClick,
}: {
	text: string;
	onClick: () => Promise<void> | void;
}) {
	const [loading, setLoading] = useState(false);

	const handleLoading = async () => {
		if (!onClick) return;

		if (loading) return;

		if (
			onClick instanceof Promise ||
			onClick.constructor.name === "AsyncFunction"
		) {
			setLoading(true);
			await onClick();
			setLoading(false);
		} else {
			onClick();
		}
	};

	return (
		<div
			className={buildClassName(
				"MainButton",
				!onClick && "disabled",
				loading && "loading",
			)}
			onClick={handleLoading}
		>
			{loading && (
				<div className="loader">
					<LoaderCircle size={20} />
				</div>
			)}
			<div className="title">{text}</div>
		</div>
	);
}

export default memo(MainButton);
