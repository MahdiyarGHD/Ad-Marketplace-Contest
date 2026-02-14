import { memo } from "react";
import type { ApplicationType } from "../stores/useApplicationStore";
import Avatar from "./Avatar";
import { Shimmer } from "./Shimmer";
import {
	AdFormats,
	PriceTypes,
	type AdFormat,
	type PriceType,
} from "../stores/useChannelStore";
import { requestAPI } from "../utils/api";
import useUIStore from "../stores/useUIStore";
import { ApplicationStatus } from "../pages/Applications";
import { buildClassName } from "../utils/common";

function ApplicationReview({
	application,
	type,
	onClose,
}: {
	application: ApplicationType;
	type: "channel" | "campaign";
	onClose: () => void;
}) {
	const { showToast } = useUIStore.getState();

	const onAccept = async () => {
		const response = await requestAPI(
			`/api/${type === "channel" ? "channel-applications" : "applications"}/${application.id}/accept`,
		);

		if (!response.isError && response.value) {
			showToast({ title: "Application accepted" });
			onClose();
		}
	};

	const onReject = async () => {
		const response = await requestAPI(
			`/api/${type === "channel" ? "channel-applications" : "applications"}/${application.id}/reject`,
		);

		if (!response.isError && response.value) {
			showToast({ title: "Application rejected" });
			onClose();
		}
	};

	return (
		<div className="ApplicationReview Profile">
			<div className="User">
				<Avatar
					id={application?.advertiser_id ?? application?.channel_id ?? ""}
					size={80}
					title={
						application?.advertiser_name ?? application?.channel_title ?? ""
					}
					isUuid
				/>
				<div className="info">
					<Shimmer className="title">
						{application.advertiser_name ?? application.channel_title}
					</Shimmer>
					<div
						className={buildClassName(
							"subtitle",
							application.status === 1 && "success",
							application.status === 2 && "destructive",
						)}
					>
						{ApplicationStatus[application.status ?? 0]}
					</div>
				</div>
			</div>

			<table className="Details">
				<tr>
					<td className="label">Ad Format</td>
					<td className="value">
						{AdFormats[application.proposed_ad_format as AdFormat]}
					</td>
				</tr>
				<tr>
					<td className="label">Price Type</td>
					<td className="value">
						{PriceTypes[application.proposed_price_type as PriceType]}
					</td>
				</tr>
				<tr>
					<td className="label">Price</td>
					<td className="value">{application.proposed_price_ton} TON</td>
				</tr>
				<tr>
					<td className="label">Message</td>
					<td className="value">{application.message}</td>
				</tr>
			</table>
			{application.status === 0 && (
				<div className="TextButton primary">
					<span>Counter Offer</span>
				</div>
			)}
			<div className="Actions">
				{application.status === 0 ? (
					<>
						<div className="Button secondary" onClick={onReject}>
							Reject
						</div>
						<div className="Button" onClick={onAccept}>
							Accept
						</div>
					</>
				) : (
					<div className="Button" onClick={onClose}>
						OK
					</div>
				)}
			</div>
		</div>
	);
}

export default memo(ApplicationReview);
