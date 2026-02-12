import { memo, useEffect, useState } from "react";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";
import { renderChannel } from "./Home";
import Transition from "../components/Transition";
import type { Channel } from "../stores/useChannelStore";
import { requestAPI } from "../utils/api";
import { useNavigate, useParams } from "react-router-dom";
import { backButton } from "@tma.js/sdk-react";
import useChannelStore from "../stores/useChannelStore";
import useCategoryStore from "../stores/useCategoryStore";

function CategoryPage() {
	const [channels, setChannels] = useState<Channel[]>([]);

	const { setActiveChannel } = useChannelStore();

	const { getCategory } = useCategoryStore();

	const { categoryId } = useParams();

	const category = getCategory(categoryId ?? "");

	const navigate = useNavigate();

	const getChannels = async () => {
		const response = await requestAPI(
			`/api/channels/by-category/${categoryId}`,
			{},
			"GET",
		);

		console.log(response);

		if (response.value?.channels) {
			setChannels(response.value.channels);
		}
	};

	const showChannelProfile = (channel: Channel) => {
		setActiveChannel(channel);

		navigate(`/channel/${channel.id}`);
	};

	const onBackButton = () => {
		window.history.back();
	};

	useEffect(() => {
		backButton.show();
		backButton.onClick(onBackButton);

		return () => {
			backButton.hide();
			backButton.offClick(onBackButton);
		};
	}, []);

	useEffect(() => {
		if (!categoryId) return onBackButton();

		getChannels();
	}, [categoryId]);

	return (
		<div className="CategoryPage">
			<PageHeader>
				<PageHeaderTitle>{category?.name}</PageHeaderTitle>
			</PageHeader>

			{channels.length !== 0 ? (
				<div className="ChatList">
					<Transition
						state
						key={channels?.length}
						eachElement
						eachElementDelay={40}
					>
						{channels.map((channel) =>
							renderChannel(channel, showChannelProfile),
						)}
					</Transition>
				</div>
			) : (
				<LoadingSkeleton />
			)}
		</div>
	);
}

const LoadingSkeleton = memo(() => (
	<div className="Skeleton">
		<div className="Items">
			{Array.from({ length: 1 }).map(() => (
				<div className="ChatItem" key={Math.random()}>
					<div className="Avatar" />
					<div className="body">
						<div className="title Shimmer" />
						<div className="subtitle Shimmer" />
					</div>
				</div>
			))}
		</div>
	</div>
));

export default memo(CategoryPage);
