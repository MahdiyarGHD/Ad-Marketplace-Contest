import { memo } from "react";
import { buildClassName } from "../utils/common";

function MainButton({ text, onClick }: { text: string; onClick: () => void }) {
	return (
		<div
			className={buildClassName("MainButton", !onClick && "disabled")}
			onClick={onClick}
		>
			<div className="title">{text}</div>
		</div>
	);
}

export default memo(MainButton);
