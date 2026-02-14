import { memo } from "react";
import { buildClassName } from "../utils/common";
import Transition from "./Transition";
import { MegaphoneIcon } from "lucide-react";

function Avatar({
	id,
	title,
	photo,
	size = 48,
	isUuid = false,
	isCampaign = false,
}: {
	id: string | number;
	title?: string;
	photo?: string;
	size?: number;
	isUuid?: boolean;
	isCampaign?: boolean;
}) {
	return (
		<div
			className={buildClassName(
				"Avatar",
				`peer-color-${getPeerColorIndexById(getPeerIdFromChatId(isCampaign || isUuid ? uuidToInt(id.toString()) : id))}`,
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
					<MegaphoneIcon size={size / 2} />
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

const uuidToInt = (uuid: string): number => parseInt(uuid.split("-")[4], 16);

export function getPeerIdFromChatId(chatId: string | number): string | number {
	return Number(chatId) < 0 ? Number(String(chatId).slice(4)) : chatId;
}

export function getPeerColorIndexById(peerId: string | number): number {
	return peerId ? Math.abs(Number(peerId)) % 7 : -1;
}

export default memo(Avatar);
