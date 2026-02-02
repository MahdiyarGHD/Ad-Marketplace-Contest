import { memo, useEffect } from "react";
import RLottie from "../components/RLottie";
import { backButton, mainButton } from "@tma.js/sdk-react";
import { invokeHapticFeedbackImpact } from "../utils/common";
import { useNavigate } from "react-router";
import "./SetChannelData.scss";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";
import Avatar from "../components/Avatar";
import { ChevronRight, DollarSignIcon, PlusIcon, TagIcon } from "lucide-react";

function SetChannelData() {
	const navigate = useNavigate();

	const onBackButton = () => {
		navigate("/my-channels");
	};

	useEffect(() => {
		mainButton.setText("Back to Channels");
		mainButton.onClick(onBackButton);
		mainButton.show();

		backButton.show();

		backButton.onClick(onBackButton);

		invokeHapticFeedbackImpact("medium");

		return () => {
			mainButton.hide();

			mainButton.offClick(onBackButton);

			backButton.hide();

			backButton.offClick(onBackButton);
		};
	}, []);

	const renderSuccess = () => {
		return (
			<div className="Placeholder">
				<div className="Emoji">
					<RLottie sticker="congrats" autoplay width={120} height={120} />
				</div>
				<h2 className="Title">Your Channel Successfully Added</h2>
				<div className="Instructions"></div>
			</div>
		);
	};

	return (
		<div className="SetChannelData">
			<PageHeader>
				<PageHeaderTitle>Create your influence channel</PageHeaderTitle>
			</PageHeader>

			<div className="Section">
				<div className="Items">
					<div className="ChatItem">
						<Avatar id="1" title="Channel 1" photo="" />
						<div className="body">
							<div className="title">Channel 1</div>
							<div className="subtitle">Subtitle 1</div>
						</div>
						<div className="meta">
							<ChevronRight />
						</div>
					</div>
					<div className="Item">
						<div className="icon">
							<TagIcon />
						</div>
						<div className="body">
							<div className="title">Select Category...</div>
						</div>
						<div className="meta">
							<ChevronRight />
						</div>
					</div>
				</div>
			</div>
			<div className="Section Pricing">
				<div className="title">Pricing</div>
				<div className="Items">
					<div className="Item">
						<div className="icon">
							<DollarSignIcon />
						</div>
						<div className="body">
							<div className="flex">
								<div className="price">
									<input type="text" placeholder="Price" />
								</div>
								<div className="price-type">Per day</div>
								<div className="ad-format">Post</div>
							</div>
						</div>
					</div>
					<div className="Item primary">
						<div className="icon">
							<PlusIcon />
						</div>
						<div className="body">
							<div className="title">Add Price...</div>
						</div>
					</div>
				</div>
				<div className="description">Set your price for sponsored content</div>
			</div>
		</div>
	);
}

export default memo(SetChannelData);
