import { memo } from "react";
import { buildClassName } from "../utils/common";
import Transition from "./Transition";
import { MegaphoneIcon } from "lucide-react";

function Avatar({
	id,
	title,
	photo,
	size = 48,
	isCampaign = false,
}: {
	id: string | number;
	title?: string;
	photo?: string;
	size?: number;
	isCampaign?: boolean;
}) {
	return (
		<div
			className={buildClassName(
				"Avatar",
				`peer-color-${getPeerColorIndexById(getPeerIdFromChatId(id))}`,
				isCampaign && "campaign",
			)}
			style={{
				width: size,
				height: size,
				fontSize: size / 2,
			}}
		>
			{isCampaign ? (
				<div className="campaign-icon">
					<MegaphoneIcon />
				</div>
			) : (
				<span>
					{title ? Array.from(title.toString())[0].toUpperCase() : ""}
				</span>
			)}
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

export function getPeerIdFromChatId(chatId: string | number): string | number {
	return Number(chatId) < 0 ? Number(String(chatId).slice(4)) : chatId;
}

export function getPeerColorIndexById(peerId: string | number): number {
	return peerId ? Math.abs(Number(peerId)) % 7 : -1;
}

export default memo(Avatar);
