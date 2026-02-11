import { memo } from "react";

function MainButton({ text, onClick }: { text: string; onClick: () => void }) {
	return (
		<div className="MainButton" onClick={onClick}>
			<div className="title">{text}</div>
		</div>
	);
}

export default memo(MainButton);
