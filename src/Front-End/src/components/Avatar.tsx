import { memo } from "react";
import { buildClassName } from "../utils/common";
import Transition from "./Transition";

function Avatar({
	id,
	title,
	photo,
	size = 48,
}: {
	id: string | number;
	title: string;
	photo: string;
	size?: number;
}) {
	return (
		<div
			className={buildClassName(
				"Avatar",
				`peer-color-${getPeerColorIndexById(id)}`,
			)}
			style={{
				width: size,
				height: size,
				fontSize: size / 2,
			}}
		>
			<span>{title ? Array.from(title.toString())[0].toUpperCase() : ""}</span>
			<Transition state={!!photo}>
				<img
					style={{ width: size, height: size }}
					draggable="false"
					decoding="async"
					src={photo}
				/>
			</Transition>
		</div>
	);
}

export function getPeerColorIndexById(peerId: string | number): number {
	return Math.abs(Number(peerId)) % 7;
}

export default memo(Avatar);
