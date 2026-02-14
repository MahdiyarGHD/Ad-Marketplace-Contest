import PageHeader, { PageHeaderTitle } from "../../components/PageHeader";
import Transition from "../../components/Transition";
import { memo, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { invokeHapticFeedbackImpact } from "../../utils/common";
import { backButton } from "@tma.js/sdk-react";
import useChannelStore, { type Channel } from "../../stores/useChannelStore";
import { renderChannel } from "../Home";
import { useShallow } from "zustand/shallow";
import useApplicationStore from "../../stores/useApplicationStore";

function SelectChannel() {
	const myChannels = useChannelStore(useShallow((state) => state.myChannels));
	const getMyChannels = useChannelStore(
		useShallow((state) => state.getMyChannels),
	);

	const applicationCampaign = useApplicationStore(
		useShallow((state) => state.application?.campaign),
	);

	const setApplication = useApplicationStore(
		useShallow((state) => state.setApplication),
	);

	const { set } = useParams();

	const navigate = useNavigate();

	const onBackButton = () => {
		window.history.back();
	};

	const onSelectChannel = (channel: Channel) => {
		switch (set) {
			case "application":
				setApplication({
					channel_id: channel.id,
					channel,
					proposed_ad_format: channel.pricings?.[0]?.ad_format,
					proposed_price_type: channel.pricings?.[0]?.price_type,
				});
				navigate(`/campaign/${applicationCampaign?.id}/apply`);
				break;
			default:
				navigate("/my-channels");
				break;
		}
	};

	useEffect(() => {
		backButton.show();

		backButton.onClick(onBackButton);

		invokeHapticFeedbackImpact("medium");

		getMyChannels();

		return () => {
			backButton.hide();

			backButton.offClick(onBackButton);
		};
	}, []);

	return (
		<div className="SelectChannel scrollable">
			<PageHeader>
				<PageHeaderTitle>Select Channel</PageHeaderTitle>
			</PageHeader>

			<div className="ChannelList Items">
				<Transition state eachElement eachElementDelay={20}>
					{myChannels.map((channel) => renderChannel(channel, onSelectChannel))}
				</Transition>
			</div>
			{myChannels.length === 0 && (
				<div className="NothingPlaceholder">
					No channels available.
					<br /> Please add a channel first.
				</div>
			)}
		</div>
	);
}

export default memo(SelectChannel);
